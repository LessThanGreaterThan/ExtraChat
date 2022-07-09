using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using Dalamud.Interface;
using Dalamud.Plugin;
using ExtraChat.Protocol;
using ExtraChat.Protocol.Channels;
using ExtraChat.Util;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Channel = System.Threading.Channels.Channel;

namespace ExtraChat.Ui;

internal class PluginUi : IDisposable {
    internal const string CrossWorld = "\ue05d";

    private Plugin Plugin { get; }

    internal bool Visible;

    private readonly List<(string, List<World>)> _worlds;
    private readonly List<(uint Id, Vector4 Abgr)> _uiColours;

    internal PluginUi(Plugin plugin) {
        this.Plugin = plugin;

        this._worlds = this.Plugin.DataManager.GetExcelSheet<World>()!
            .Where(row => row.IsPublic)
            .GroupBy(row => row.DataCenter.Value!)
            .Where(grouping => grouping.Key.Region != 0)
            .OrderBy(grouping => grouping.Key.Region)
            .ThenBy(grouping => grouping.Key.Name.RawString)
            .Select(grouping => (grouping.Key.Name.RawString, grouping.OrderBy(row => row.Name.RawString).ToList()))
            .ToList();

        this._uiColours = this.Plugin.DataManager.GetExcelSheet<UIColor>()!
            .Where(row => row.UIForeground is not (0 or 0x000000FF))
            .Select(row => (row.RowId, row.UIForeground, ColourUtil.Step(row.UIForeground)))
            .GroupBy(row => row.UIForeground)
            .Select(grouping => grouping.First())
            .OrderBy(row => row.Item3.Item1)
            .ThenBy(row => row.Item3.Item2)
            .ThenBy(row => row.Item3.Item3)
            .Select(row => (row.RowId, ImGui.ColorConvertU32ToFloat4(ColourUtil.RgbaToAbgr(row.Item2))))
            .ToList();

        this.Plugin.Interface.UiBuilder.Draw += this.Draw;
        this.Plugin.Interface.UiBuilder.OpenConfigUi += this.OpenConfigUi;

        if (this.Plugin.Interface.Reason == PluginLoadReason.Installer && this.Plugin.ConfigInfo.Key == null) {
            this.Visible = true;
        }
    }

    public void Dispose() {
        this.Plugin.Interface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;
        this.Plugin.Interface.UiBuilder.Draw -= this.Draw;
    }

    private void OpenConfigUi() {
        this.Visible ^= true;
    }

    internal (string, ushort)? InviteInfo;

    private volatile bool _busy;
    private string? _challenge;
    private string _createName = string.Empty;
    private Guid? _inviteId;
    private readonly Channel<string?> _challengeChannel = Channel.CreateUnbounded<string?>();

    private void Draw() {
        if (this._challengeChannel.Reader.TryRead(out var challenge)) {
            this._challenge = challenge;
        }

        this.DrawConfigWindow();
        this.DrawInviteWindow();
    }

    private void DrawConfigWindow() {
        if (!this.Visible) {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(500, 325) * ImGuiHelpers.GlobalScale, ImGuiCond.FirstUseEver);

        if (!ImGui.Begin(this.Plugin.Name, ref this.Visible)) {
            ImGui.End();
            return;
        }

        if (!this.Plugin.ClientState.IsLoggedIn) {
            ImGui.TextUnformatted("Please log in to a character.");
            ImGui.End();
            return;
        }

        if (ImGui.BeginTabBar("tabs")) {
            if (ImGui.BeginTabItem("Linkshells")) {
                var status = this.Plugin.Client.Status;
                ImGui.TextUnformatted($"Status: {status}");

                switch (status) {
                    case Client.State.Connected:
                        this.DrawList();
                        break;
                    case Client.State.NotAuthenticated:
                    case Client.State.RetrievingChallenge:
                    case Client.State.WaitingForVerification:
                    case Client.State.Verifying:
                        this.DrawRegistrationPanel();
                        break;
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Settings")) {
                this.DrawSettings();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Help")) {
                this.DrawHelp();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        ImGui.End();
    }

    private void DrawHelp() {
        ImGui.PushTextWrapPos();

        if (ImGui.Button("Reset tutorial")) {
            this.Plugin.ConfigInfo.TutorialStep = 0;
            this.Plugin.SaveConfig();
        }

        ImGui.PopTextWrapPos();
    }

    private void DrawSettings() {
        var anyChanged = false;

        if (ImGui.BeginTabBar("settings-tabs")) {
            if (ImGui.BeginTabItem("General")) {
                this.DrawSettingsGeneral(ref anyChanged);
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Linkshells")) {
                this.DrawSettingsLinkshells(ref anyChanged);
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        if (anyChanged) {
            this.Plugin.SaveConfig();
            this.Plugin.Ipc.BroadcastChannelCommandColours();
        }
    }

    private void DrawSettingsGeneral(ref bool anyChanged) {
        anyChanged |= ImGui.Checkbox("Use native toasts", ref this.Plugin.Config.UseNativeToasts);
        // ImGui.Spacing();
        //
        // ImGui.TextUnformatted("Default channel");
        // ImGui.SetNextItemWidth(-1);
        // if (ImGui.BeginCombo("##default-channel", $"{this.Plugin.Config.DefaultChannel}")) {
        //     foreach (var channel in Enum.GetValues<XivChatType>()) {
        //         if (ImGui.Selectable($"{channel}", this.Plugin.Config.DefaultChannel == channel)) {
        //             this.Plugin.Config.DefaultChannel = channel;
        //             anyChanged = true;
        //         }
        //     }
        //
        //     ImGui.EndCombo();
        // }
    }

    private void DrawSettingsLinkshells(ref bool anyChanged) {
        var channelOrder = this.Plugin.ConfigInfo.ChannelOrder.ToDictionary(
            entry => entry.Value,
            entry => entry.Key
        );

        var orderedChannels = this.Plugin.Client.Channels.Keys
            .OrderBy(id => channelOrder.ContainsKey(id) ? channelOrder[id] : int.MaxValue)
            .Concat(this.Plugin.Client.InvitedChannels.Keys);

        foreach (var id in orderedChannels) {
            var name = this.Plugin.ConfigInfo.GetName(id);

            if (ImGui.CollapsingHeader($"{name}###{id}-settings")) {
                ImGui.PushID($"{id}-settings");

                ImGui.TextUnformatted("Number");
                channelOrder.TryGetValue(id, out var refOrder);
                var old = refOrder;
                refOrder += 1;
                ImGui.SetNextItemWidth(-1);
                if (ImGui.InputInt("##order", ref refOrder)) {
                    refOrder = Math.Max(1, refOrder) - 1;

                    if (this.Plugin.ConfigInfo.ChannelOrder.TryGetValue(refOrder, out var other) && other != id) {
                        // another channel already has this number, so swap
                        this.Plugin.ConfigInfo.ChannelOrder[old] = other;
                    } else {
                        this.Plugin.ConfigInfo.ChannelOrder.Remove(old);
                    }

                    this.Plugin.ConfigInfo.ChannelOrder[refOrder] = id;
                    anyChanged = true;
                    this.Plugin.Commands.ReregisterAll();
                }

                ImGui.Spacing();

                if (ImGuiUtil.IconButton(FontAwesomeIcon.Undo, "colour-reset", "Reset")) {
                    anyChanged = true;
                    this.Plugin.ConfigInfo.ChannelColors.Remove(id);
                }

                ImGui.SameLine();

                var colourKey = this.Plugin.ConfigInfo.GetUiColour(id);
                var colour = this.Plugin.DataManager.GetExcelSheet<UIColor>()!.GetRow(colourKey)?.UIForeground ?? 0xff5ad0ff;
                var vec = ImGui.ColorConvertU32ToFloat4(ColourUtil.RgbaToAbgr(colour));

                const string colourPickerId = "linkshell-colour-picker";

                if (ImGui.ColorButton("Linkshell colour", vec, ImGuiColorEditFlags.NoTooltip)) {
                    ImGui.OpenPopup(colourPickerId);
                }

                ImGui.SameLine();

                ImGui.TextUnformatted("Linkshell colour");

                if (ImGui.BeginPopup(colourPickerId)) {
                    var i = 0;

                    foreach (var (uiColour, fg) in this._uiColours) {
                        if (ImGui.ColorButton($"Colour {uiColour}", fg, ImGuiColorEditFlags.NoTooltip)) {
                            this.Plugin.ConfigInfo.ChannelColors[id] = (ushort) uiColour;
                            anyChanged = true;
                            ImGui.CloseCurrentPopup();
                        }

                        if (i >= 11) {
                            i = 0;
                        } else {
                            ImGui.SameLine();
                            i += 1;
                        }
                    }

                    ImGui.EndPopup();
                }

                ImGui.Spacing();

                var hint = $"ECLS{refOrder}";
                if (!this.Plugin.ConfigInfo.ChannelMarkers.TryGetValue(id, out var marker)) {
                    marker = string.Empty;
                }

                ImGui.TextUnformatted("Chat marker");
                ImGui.SetNextItemWidth(-1);
                if (ImGui.InputTextWithHint("##marker", hint, ref marker, 16)) {
                    anyChanged = true;
                    if (string.IsNullOrWhiteSpace(marker)) {
                        this.Plugin.ConfigInfo.ChannelMarkers.Remove(id);
                    } else {
                        this.Plugin.ConfigInfo.ChannelMarkers[id] = marker;
                    }
                }

                // ImGui.Spacing();
                //
                // ImGui.TextUnformatted("Output channel");
                // ImGui.SetNextItemWidth(-1);
                //
                // var contained = this.Plugin.ConfigInfo.ChannelChannels.TryGetValue(id, out var output);
                // var preview = contained ? $"{output}" : "Default";
                //
                // if (ImGui.BeginCombo("##output-channel", preview)) {
                //     if (ImGui.Selectable("Default", !contained)) {
                //         this.Plugin.ConfigInfo.ChannelChannels.Remove(id);
                //         anyChanged = true;
                //     }
                //
                //     foreach (var channel in Enum.GetValues<XivChatType>()) {
                //         if (ImGui.Selectable($"{channel}", contained && output == channel)) {
                //             this.Plugin.ConfigInfo.ChannelChannels[id] = channel;
                //             anyChanged = true;
                //         }
                //     }
                //
                //     ImGui.EndCombo();
                // }

                ImGui.PopID();
            }
        }
    }

    private void DrawInviteWindow() {
        if (this.InviteInfo == null) {
            return;
        }

        var (name, world) = this.InviteInfo.Value;

        var open = true;
        if (!ImGui.Begin($"Invite: {name}###ec-linkshell-invite", ref open, ImGuiWindowFlags.AlwaysAutoResize)) {
            if (!open) {
                this.InviteInfo = null;
            }

            ImGui.End();
            return;
        }

        if (!open) {
            this.InviteInfo = null;
        }

        if (ImGui.IsWindowAppearing()) {
            ImGui.SetWindowPos(ImGui.GetMousePos());
        }

        var preview = this._inviteId == null ? "Choose a linkshell" : "???";
        if (this._inviteId != null && this.Plugin.ConfigInfo.Channels.TryGetValue(this._inviteId.Value, out var selectedInfo)) {
            preview = selectedInfo.Name;
        }

        if (ImGui.BeginCombo("##ec-linkshell-invite-linkshell", preview)) {
            foreach (var (id, _) in this.Plugin.Client.Channels) {
                if (!this.Plugin.Client.ChannelRanks.TryGetValue(id, out var rank) || rank < Rank.Moderator) {
                    continue;
                }

                if (!this.Plugin.ConfigInfo.Channels.TryGetValue(id, out var info)) {
                    continue;
                }

                if (ImGui.Selectable($"{info.Name}##{id}", id == this._inviteId)) {
                    this._inviteId = id;
                }
            }

            ImGui.EndCombo();
        }

        if (ImGui.Button("Invite") && this._inviteId != null) {
            var id = this._inviteId.Value;
            this._inviteId = null;

            Task.Run(async () => await this.Plugin.Client.InviteToast(name, world, id));
            this.InviteInfo = null;
        }

        ImGui.End();
    }

    private void DrawRegistrationPanel() {
        if (this.Plugin.LocalPlayer is not { } player) {
            return;
        }

        var state = this.Plugin.Client.Status;
        if (state == Client.State.NotAuthenticated) {
            if (this.Plugin.ConfigInfo.Key != null) {
                ImGui.TextUnformatted("Please wait...");
            } else {
                if (ImGui.Button($"Register {player.Name}") && !this._busy) {
                    this._busy = true;
                    Task.Run(async () => {
                        var challenge = await this.Plugin.Client.GetChallenge();
                        await this._challengeChannel.Writer.WriteAsync(challenge);
                    }).ContinueWith(_ => this._busy = false);
                }

                ImGui.PushTextWrapPos();
                ImGui.TextUnformatted("ExtraChat is a third-party service that allows for functionally unlimited extra linkshells that work across data centres.");
                ImGui.TextUnformatted("In order to use ExtraChat, characters must be registered and verified using their Lodestone profile.");
                ImGui.TextUnformatted("ExtraChat stores your character's name, home world, and Lodestone ID, as well as what linkshells your character is a part of and has been invited to.");
                ImGui.TextUnformatted("Messages and linkshell names are end-to-end encrypted; the server cannot decrypt them and does not store messages.");
                ImGui.TextUnformatted("In the event of a legal subpoena, ExtraChat will provide any information available to the legal system.");
                ImGui.PopTextWrapPos();
            }
        }

        if (state == Client.State.RetrievingChallenge) {
            ImGui.TextUnformatted("Waiting...");
        }

        if (state == Client.State.WaitingForVerification) {
            ImGui.PushTextWrapPos();
            if (this._challenge == null) {
                ImGui.TextUnformatted("Waiting for verification but no challenge present. This is a bug.");
            } else {
                ImGui.TextUnformatted("Copy the challenge below and save it in your Lodestone profile. After saving, click the button below to verify. After successfully verifying, you can delete the challenge from your profile if desired.");

                ImGui.SetNextItemWidth(-1);
                ImGui.InputText("##challenge", ref this._challenge, (uint) this._challenge.Length, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.ReadOnly);

                if (ImGui.Button("Copy")) {
                    ImGui.SetClipboardText(this._challenge);
                }

                ImGui.SameLine();

                if (ImGui.Button("Open profile")) {
                    Process.Start(new ProcessStartInfo {
                        FileName = "https://na.finalfantasyxiv.com/lodestone/my/setting/profile/",
                        UseShellExecute = true,
                    });
                }

                ImGui.SameLine();

                if (ImGui.Button("Verify") && !this._busy) {
                    this._busy = true;
                    Task.Run(async () => {
                        var key = await this.Plugin.Client.Register();
                        this.Plugin.ConfigInfo.Key = key;
                        this.Plugin.SaveConfig();
                        await this.Plugin.Client.AuthenticateAndList();
                    }).ContinueWith(_ => this._busy = false);
                }
            }

            ImGui.PopTextWrapPos();
        }
    }

    private Guid _selectedChannel = Guid.Empty;
    private string _inviteName = string.Empty;
    private ushort _inviteWorld;
    private string _rename = string.Empty;

    private void DrawList() {
        var anyChanged = false;

        ImGui.PushFont(UiBuilder.IconFont);

        var syncButton = ImGui.CalcTextSize(FontAwesomeIcon.Sync.ToIconString()).X
                         + ImGui.GetStyle().FramePadding.X * 2;
        // PluginLog.Log($"syncButton: {syncButton}");
        var addButton = ImGui.CalcTextSize(FontAwesomeIcon.Plus.ToIconString()).X
                        + ImGui.GetStyle().FramePadding.X * 2;
        // PluginLog.Log($"addButton: {addButton}");
        var syncOffset = ImGui.GetContentRegionAvail().X - syncButton;
        var addOffset = ImGui.GetContentRegionAvail().X - syncButton - ImGui.GetStyle().ItemSpacing.X - addButton;
        ImGui.SameLine(syncOffset);

        if (ImGui.Button(FontAwesomeIcon.Sync.ToIconString())) {
            Task.Run(async () => await this.Plugin.Client.ListAll());
        }

        anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 1);

        ImGui.SameLine(addOffset);

        if (ImGui.Button(FontAwesomeIcon.Plus.ToIconString())) {
            ImGui.OpenPopup("create-channel-popup");
        }

        anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 0);

        ImGui.PopFont();

        if (ImGui.BeginPopup("create-channel-popup")) {
            ImGui.TextUnformatted("Create a new ExtraChat Linkshell");

            ImGui.SetNextItemWidth(350 * ImGuiHelpers.GlobalScale);
            ImGui.InputTextWithHint("##linkshell-name", "Linkshell name", ref this._createName, 64);

            if (ImGui.IsWindowAppearing()) {
                ImGui.SetKeyboardFocusHere();
            }

            if (!string.IsNullOrWhiteSpace(this._createName) && ImGui.Button("Create") && !this._busy) {
                this._busy = true;
                var name = this._createName;
                Task.Run(async () => await this.Plugin.Client.Create(name))
                    .ContinueWith(_ => this._busy = false);
                ImGui.CloseCurrentPopup();
                this._createName = string.Empty;
            }

            ImGui.EndPopup();
        }

        if (this.Plugin.Client.Channels.Count == 0) {
            ImGui.TextUnformatted("You aren't in any linkshells yet. Try creating or joining one first.");
            goto AfterTable;
        }

        if (ImGui.BeginTable("ecls-list", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit)) {
            ImGui.TableSetupColumn("##channels", ImGuiTableColumnFlags.WidthFixed, 125 * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("##members", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();

            var channelOrder = this.Plugin.ConfigInfo.ChannelOrder.ToDictionary(
                entry => entry.Value,
                entry => entry.Key
            );

            var orderedChannels = this.Plugin.Client.Channels.Keys
                .OrderBy(id => channelOrder.ContainsKey(id) ? channelOrder[id] : int.MaxValue)
                .Concat(this.Plugin.Client.InvitedChannels.Keys);

            var childSize = new Vector2(
                -1,
                ImGui.GetContentRegionAvail().Y
                - ImGui.GetStyle().WindowPadding.Y
                - ImGui.GetStyle().ItemSpacing.Y
            );

            if (ImGui.TableSetColumnIndex(0)) {
                if (ImGui.BeginChild("channel-list", childSize)) {
                    var first = true;
                    foreach (var id in orderedChannels) {
                        this.Plugin.ConfigInfo.Channels.TryGetValue(id, out var info);
                        var name = info?.Name ?? "???";

                        var order = "?";
                        if (channelOrder.TryGetValue(id, out var o)) {
                            order = (o + 1).ToString();
                        }

                        if (!this.Plugin.Client.ChannelRanks.TryGetValue(id, out var rank)) {
                            rank = Rank.Member;
                        }

                        if (ImGui.Selectable($"{order}. {rank.Symbol()}{name}###{id}", this._selectedChannel == id)) {
                            this._selectedChannel = id;

                            Task.Run(async () => await this.Plugin.Client.ListMembers(id));
                        }

                        if (first) {
                            first = false;
                            anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 2);
                            anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 3);
                        }

                        if (ImGui.BeginPopupContextItem()) {
                            var invited = this.Plugin.Client.InvitedChannels.ContainsKey(id);
                            if (invited) {
                                if (ImGui.Selectable("Accept invite")) {
                                    Task.Run(async () => await this.Plugin.Client.Join(id));
                                }

                                if (ImGuiUtil.SelectableConfirm("Decline invite")) {
                                    Task.Run(async () => await this.Plugin.Client.Leave(id));
                                }
                            } else {
                                if (ImGuiUtil.SelectableConfirm("Leave")) {
                                    Task.Run(async () => await this.Plugin.Client.Leave(id));
                                }

                                if (rank == Rank.Admin) {
                                    if (ImGuiUtil.SelectableConfirm("Disband")) {
                                        Task.Run(async () => {
                                            if (await this.Plugin.Client.Disband(id) is { } error) {
                                                this.Plugin.ShowError($"Could not disband \"{name}\": {error}");
                                            }
                                        });
                                    }
                                }

                                if (rank == Rank.Admin && info != null && ImGui.BeginMenu($"Rename##{id}-rename")) {
                                    if (ImGui.IsWindowAppearing()) {
                                        this._rename = string.Empty;
                                    }

                                    ImGui.SetNextItemWidth(350 * ImGuiHelpers.GlobalScale);
                                    ImGui.InputTextWithHint($"##{id}-rename-input", "New name", ref this._rename, 64);

                                    if (ImGui.IsWindowAppearing()) {
                                        ImGui.SetKeyboardFocusHere();
                                    }

                                    if (ImGui.Button($"Rename##{id}-rename-button") && !string.IsNullOrWhiteSpace(this._rename)) {
                                        var newName = SecretBox.Encrypt(info.SharedSecret, Encoding.UTF8.GetBytes(this._rename));
                                        Task.Run(async () => await this.Plugin.Client.UpdateToast(id, new UpdateKind.Name(newName)));
                                        ImGui.CloseCurrentPopup();
                                    }

                                    ImGui.EndMenu();
                                }

                                if (ImGui.BeginMenu($"Invite##{id}-invite")) {
                                    if (ImGui.IsWindowAppearing()) {
                                        this._inviteName = string.Empty;
                                        this._inviteWorld = 0;
                                    }

                                    ImGui.SetNextItemWidth(250 * ImGuiHelpers.GlobalScale);
                                    ImGui.InputTextWithHint("##invite-name", "Name", ref this._inviteName, 32);

                                    ImGui.SetNextItemWidth(250 * ImGuiHelpers.GlobalScale);
                                    var preview = this._inviteWorld == 0 ? "World" : WorldUtil.WorldName(this._inviteWorld);
                                    if (ImGui.BeginCombo("##invite-world", preview)) {
                                        foreach (var (dc, worlds) in this._worlds) {
                                            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetStyle().Colors[(int) ImGuiCol.TextDisabled]);
                                            ImGui.TextUnformatted(dc);
                                            ImGui.PopStyleColor();
                                            ImGui.Separator();

                                            foreach (var world in worlds) {
                                                if (ImGui.Selectable(world.Name.RawString, this._inviteWorld == world.RowId)) {
                                                    this._inviteWorld = (ushort) world.RowId;
                                                }
                                            }

                                            ImGui.Spacing();
                                        }

                                        ImGui.EndCombo();
                                    }

                                    if (ImGui.Button($"Invite##{id}-invite-button") && !string.IsNullOrWhiteSpace(this._inviteName) && this._inviteWorld != 0) {
                                        var inviteName = this._inviteName;
                                        var inviteWorld = this._inviteWorld;

                                        Task.Run(async () => await this.Plugin.Client.InviteToast(inviteName, inviteWorld, id));
                                    }

                                    ImGui.EndMenu();
                                }

                                ImGui.Separator();

                                if (ImGui.BeginMenu("Change number")) {
                                    ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
                                    channelOrder.TryGetValue(id, out var refOrder);
                                    var old = refOrder;
                                    refOrder += 1;
                                    if (ImGui.InputInt($"##{id}-order", ref refOrder)) {
                                        refOrder = Math.Max(1, refOrder) - 1;

                                        if (this.Plugin.ConfigInfo.ChannelOrder.TryGetValue(refOrder, out var other) && other != id) {
                                            // another channel already has this number, so swap
                                            this.Plugin.ConfigInfo.ChannelOrder[old] = other;
                                        } else {
                                            this.Plugin.ConfigInfo.ChannelOrder.Remove(old);
                                        }

                                        this.Plugin.ConfigInfo.ChannelOrder[refOrder] = id;
                                        this.Plugin.SaveConfig();
                                        this.Plugin.Commands.ReregisterAll();
                                    }

                                    ImGui.EndMenu();
                                }

                                if (info == null) {
                                    if (ImGui.Selectable("Request secrets")) {
                                        Task.Run(async () => await this.Plugin.Client.RequestSecrets(id));
                                    }
                                }
                            }

                            ImGui.EndPopup();
                        }
                    }

                    ImGui.EndChild();
                }
            }

            if (ImGui.TableSetColumnIndex(1) && this._selectedChannel != Guid.Empty) {
                void DrawInfo() {
                    if (!this.Plugin.Client.TryGetChannel(this._selectedChannel, out var channel)) {
                        return;
                    }

                    Vector4 disabledColour;
                    unsafe {
                        disabledColour = *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled);
                    }

                    if (!this.Plugin.Client.ChannelRanks.TryGetValue(this._selectedChannel, out var rank)) {
                        rank = Rank.Member;
                    }

                    var first = true;
                    foreach (var member in channel.Members) {
                        if (!member.Online) {
                            ImGui.PushStyleColor(ImGuiCol.Text, disabledColour);
                        }

                        try {
                            ImGui.TextUnformatted($"{member.Rank.Symbol()}{member.Name}{CrossWorld}{WorldUtil.WorldName(member.World)}");
                        } finally {
                            if (!member.Online) {
                                ImGui.PopStyleColor();
                            }
                        }

                        if (first) {
                            first = false;
                            anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 4);
                            anyChanged |= ImGuiUtil.Tutorial(this.Plugin, 5);
                        }

                        if (ImGui.BeginPopupContextItem($"{this._selectedChannel}-{member.Name}@{member.World}-context")) {
                            if (rank == Rank.Admin) {
                                if (member.Rank is not (Rank.Admin or Rank.Invited)) {
                                    if (ImGuiUtil.SelectableConfirm("Promote to admin", tooltip: "This will demote you to moderator.")) {
                                        Task.Run(async () => await this.Plugin.Client.Promote(this._selectedChannel, member.Name, member.World, Rank.Admin));
                                    }
                                }

                                if (member.Rank == Rank.Moderator && ImGuiUtil.SelectableConfirm("Demote")) {
                                    Task.Run(async () => await this.Plugin.Client.Promote(this._selectedChannel, member.Name, member.World, Rank.Member));
                                }

                                if (member.Rank == Rank.Member && ImGuiUtil.SelectableConfirm("Promote to moderator")) {
                                    Task.Run(async () => await this.Plugin.Client.Promote(this._selectedChannel, member.Name, member.World, Rank.Moderator));
                                }
                            }

                            if (rank >= Rank.Moderator) {
                                var canKick = member.Rank < rank && member.Rank != Rank.Invited;
                                if (canKick && ImGuiUtil.SelectableConfirm("Kick")) {
                                    Task.Run(async () => {
                                        if (await this.Plugin.Client.Kick(this._selectedChannel, member.Name, member.World) is { } error) {
                                            this.Plugin.ShowError($"Could not kick {member.Name}: {error}");
                                        }
                                    });
                                }

                                if (member.Rank == Rank.Invited && ImGuiUtil.SelectableConfirm("Cancel invite")) {
                                    Task.Run(async () => await this.Plugin.Client.Kick(this._selectedChannel, member.Name, member.World));
                                }
                            }

                            if (rank == Rank.Invited && member.Rank == Rank.Invited) {
                                if (member.Name == this.Plugin.LocalPlayer?.Name.TextValue && member.World == this.Plugin.LocalPlayer?.HomeWorld.Id) {
                                    if (ImGui.Selectable("Accept invite")) {
                                        Task.Run(async () => await this.Plugin.Client.Join(this._selectedChannel));
                                    }

                                    if (ImGuiUtil.SelectableConfirm("Decline invite")) {
                                        Task.Run(async () => await this.Plugin.Client.Leave(this._selectedChannel));
                                    }
                                }
                            }


                            ImGui.EndPopup();
                        }
                    }
                }

                if (ImGui.BeginChild("channel-info", childSize)) {
                    DrawInfo();
                    ImGui.EndChild();
                }
            }

            ImGui.EndTable();
        }

        AfterTable:
        if (anyChanged) {
            this.Plugin.SaveConfig();
        }
    }
}
