using ASodium;
using Dalamud.ContextMenu;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.IoC;
using Dalamud.Plugin;
using ExtraChat.Integrations;
using ExtraChat.Ui;
using ExtraChat.Util;

namespace ExtraChat;

// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : IDalamudPlugin {
    internal const string PluginName = "ExtraChat";
    internal const ushort DefaultColour = 578;

    public string Name => PluginName;

    [PluginService]
    internal DalamudPluginInterface Interface { get; init; }

    [PluginService]
    internal ClientState ClientState { get; init; }

    [PluginService]
    internal CommandManager CommandManager { get; init; }

    [PluginService]
    internal ChatGui ChatGui { get; init; }

    [PluginService]
    internal DataManager DataManager { get; init; }

    [PluginService]
    internal Framework Framework { get; init; }

    [PluginService]
    internal GameGui GameGui { get; init; }

    [PluginService]
    internal ObjectTable ObjectTable { get; init; }

    [PluginService]
    internal TargetManager TargetManager { get; init; }

    [PluginService]
    private ToastGui ToastGui { get; init; }

    internal Configuration Config { get; }
    internal ConfigInfo ConfigInfo => this.Config.GetConfig(this.ClientState.LocalContentId);
    internal Client Client { get; }
    internal Commands Commands { get; }
    internal PluginUi PluginUi { get; }
    internal DalamudContextMenu ContextMenu { get; }
    internal GameFunctions GameFunctions { get; }
    internal Ipc Ipc { get; }
    private IDisposable[] Integrations { get; }

    private PlayerCharacter? _localPlayer;
    private readonly Mutex _localPlayerLock = new();

    internal PlayerCharacter? LocalPlayer {
        get {
            this._localPlayerLock.WaitOne();
            var player = this._localPlayer;
            this._localPlayerLock.ReleaseMutex();
            return player;
        }
        private set {
            this._localPlayerLock.WaitOne();
            this._localPlayer = value;
            this._localPlayerLock.ReleaseMutex();
        }
    }

    public Plugin() {
        SodiumInit.Init();
        WorldUtil.Initialise(this.DataManager!);
        this.ContextMenu = new DalamudContextMenu();
        this.Config = this.Interface!.GetPluginConfig() as Configuration ?? new Configuration();
        this.Client = new Client(this);
        this.Commands = new Commands(this);
        this.PluginUi = new PluginUi(this);
        this.GameFunctions = new GameFunctions(this);
        this.Ipc = new Ipc(this);

        this.Integrations = new IDisposable[] {
            new ChatTwo(this),
        };

        this.Framework!.Update += this.FrameworkUpdate;
        this.ContextMenu.OnOpenGameObjectContextMenu += this.OnOpenGameObjectContextMenu;
    }

    public void Dispose() {
        this.GameFunctions.ResetOverride();

        this.ContextMenu.OnOpenGameObjectContextMenu -= this.OnOpenGameObjectContextMenu;
        this.Framework.Update -= this.FrameworkUpdate;
        this._localPlayerLock.Dispose();

        foreach (var integration in this.Integrations) {
            integration.Dispose();
        }

        this.Ipc.Dispose();
        this.GameFunctions.Dispose();
        this.PluginUi.Dispose();
        this.Commands.Dispose();
        this.Client.Dispose();
        this.ContextMenu.Dispose();
    }

    private void FrameworkUpdate(Framework framework) {
        if (this.ClientState.LocalPlayer is { } player) {
            this.LocalPlayer = player;
        } else if (!this.ClientState.IsLoggedIn) {
            // only set to null if not logged in
            this.LocalPlayer = null;
        }
    }

    private void OnOpenGameObjectContextMenu(GameObjectContextMenuOpenArgs args) {
        if (!this.Config.ShowContextMenuItem) {
            return;
        }

        if (args.ObjectId != 0xE0000000) {
            this.ObjectContext(args);
            return;
        }

        if (args.ObjectWorld == 0) {
            return;
        }

        var name = args.Text?.TextValue;
        if (name == null) {
            return;
        }

        args.AddCustomItem(new GameObjectContextMenuItem("Invite to ExtraChat Linkshell", _ => {
            this.PluginUi.InviteInfo = (name, args.ObjectWorld);
        }));
    }

    private void ObjectContext(GameObjectContextMenuOpenArgs args) {
        var obj = this.ObjectTable.SearchById(args.ObjectId);
        if (obj is not PlayerCharacter chara) {
            return;
        }

        args.AddCustomItem(new GameObjectContextMenuItem("Invite to ExtraChat Linkshell", _ => {
            var name = chara.Name.TextValue;
            this.PluginUi.InviteInfo = (name, (ushort) chara.HomeWorld.Id);
        }));
    }

    internal void SaveConfig() {
        this.Interface.SavePluginConfig(this.Config);
    }

    internal void ShowInfo(string message) {
        if (this.Config.UseNativeToasts) {
            this.ToastGui.ShowNormal(message);
        } else {
            this.Interface.UiBuilder.AddNotification(message, this.Name, NotificationType.Info);
        }

        this.ChatGui.PrintChat(new XivChatEntry {
            Type = XivChatType.SystemMessage,
            Message = message,
        });
    }

    internal void ShowError(string message) {
        if (this.Config.UseNativeToasts) {
            this.ToastGui.ShowError(message);
        } else {
            this.Interface.UiBuilder.AddNotification(message, this.Name, NotificationType.Error);
        }

        this.ChatGui.PrintChat(new XivChatEntry {
            Type = XivChatType.ErrorMessage,
            Message = message,
        });
    }
}
