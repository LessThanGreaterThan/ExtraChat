using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace ExtraChat.Util;

internal static class PayloadUtil {
    internal static RawPayload CreateTagPayload(Guid id) {
        var bytes = new List<byte> {
            2, // start byte
            0x27, // interactable
            3 + 16, // chunk length (3 bytes plus data length)
            0x20, // embedded info type (custom)
        };

        // now add data, we always know it's 16 bytes
        bytes.AddRange(id.ToByteArray());

        // end byte
        bytes.Add(3);

        return new RawPayload(bytes.ToArray());
    }
}
