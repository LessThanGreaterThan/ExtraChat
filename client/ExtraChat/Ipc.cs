using Dalamud.Plugin.Ipc;
using Lumina.Excel.GeneratedSheets;

namespace ExtraChat;

internal class Ipc : IDisposable {
    [Serializable]
    private struct OverrideInfo {
        public string? Channel;
        public ushort UiColour;
        public uint Rgba;
    }

    private Plugin Plugin { get; }
    private ICallGateProvider<OverrideInfo, object> OverrideChannelColour { get; }
    private ICallGateProvider<Dictionary<string, uint>, Dictionary<string, uint>> ChannelCommandColours { get; }

    internal Ipc(Plugin plugin) {
        this.Plugin = plugin;

        this.OverrideChannelColour = this.Plugin.Interface.GetIpcProvider<OverrideInfo, object>("ExtraChat.OverrideChannelColour");
        this.ChannelCommandColours = this.Plugin.Interface.GetIpcProvider<Dictionary<string, uint>, Dictionary<string, uint>>("ExtraChat.ChannelCommandColours");

        this.ChannelCommandColours.RegisterFunc(_ => this.GetChannelColours());
    }

    public void Dispose() {
        this.ChannelCommandColours.UnregisterFunc();
    }

    private Dictionary<string, uint> GetChannelColours() {
        var dict = new Dictionary<string, uint>(this.Plugin.Commands.Registered.Count);

        foreach (var (command, id) in this.Plugin.Commands.Registered) {
            var colour = this.Plugin.ConfigInfo.GetUiColour(id);
            if (this.Plugin.DataManager.GetExcelSheet<UIColor>()?.GetRow(colour)?.UIForeground is { } rgba) {
                dict[command] = rgba;
            }
        }

        return dict;
    }

    internal void BroadcastChannelCommandColours() {
        this.ChannelCommandColours.SendMessage(this.GetChannelColours());
    }

    internal void BroadcastOverride() {
        var over = this.Plugin.GameFunctions.OverrideChannel;
        if (over == Guid.Empty) {
            this.OverrideChannelColour.SendMessage(new OverrideInfo());
            return;
        }

        var name = this.Plugin.ConfigInfo.GetFullName(over);
        var colour = this.Plugin.ConfigInfo.GetUiColour(over);
        var rgba = this.Plugin.DataManager.GetExcelSheet<UIColor>()?.GetRow(colour)?.UIForeground ?? 0;

        this.OverrideChannelColour.SendMessage(new OverrideInfo {
            Channel = name,
            UiColour = colour,
            Rgba = rgba,
        });
    }
}
