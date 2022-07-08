using System.Text;
using Dalamud.Interface;
using ImGuiNET;

namespace ExtraChat.Util;

internal static class ImGuiUtil {
    internal static bool IconButton(FontAwesomeIcon icon, string? id = null, string? tooltip = null) {
        var label = icon.ToIconString();
        if (id != null) {
            label += $"##{id}";
        }

        ImGui.PushFont(UiBuilder.IconFont);
        var ret = ImGui.Button(label);
        ImGui.PopFont();

        if (tooltip != null && ImGui.IsItemHovered()) {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(tooltip);
            ImGui.EndTooltip();
        }

        return ret;
    }

    internal static bool SelectableConfirm(string label, ConfirmKey keys = ConfirmKey.Ctrl, string? tooltip = null) {
        var selectable = ImGui.Selectable(label);
        var hovered = ImGui.IsItemHovered();

        var confirmHeld = true;
        var mods = hovered ? new StringBuilder() : null;
        foreach (var key in Enum.GetValues<ConfirmKey>()) {
            if (!keys.HasFlag(key)) {
                continue;
            }

            if (hovered) {
                if (mods!.Length != 0) {
                    mods.Append('+');
                }

                mods.Append(key.ToString());
            }

            var held = key switch {
                ConfirmKey.Ctrl => ImGui.GetIO().KeyCtrl,
                ConfirmKey.Alt => ImGui.GetIO().KeyAlt,
                ConfirmKey.Shift => ImGui.GetIO().KeyShift,
                _ => false,
            };
            confirmHeld &= held;
        }

        if (!confirmHeld && hovered) {
            ImGui.BeginTooltip();
            var explainer = $"Hold {mods} to enable this option.";
            var tip = tooltip == null ? explainer : $"{tooltip}\n{explainer}";
            ImGui.TextUnformatted(tip);
            ImGui.EndTooltip();
        }

        return selectable && confirmHeld;
    }
}

[Flags]
internal enum ConfirmKey {
    Ctrl = 1 << 0,
    Alt = 1 << 1,
    Shift = 1 << 2,
}
