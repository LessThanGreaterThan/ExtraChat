using System.Net.WebSockets;
using ExtraChat.Protocol;
using MessagePack;

namespace ExtraChat;

public static class Ext {
    public static string ToHexString(this IEnumerable<byte> bytes) {
        return string.Join("", bytes.Select(b => b.ToString("x2")));
    }

    public static async Task SendMessage(this ClientWebSocket client, RequestContainer request) {
        var bytes = MessagePackSerializer.Serialize(request);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.SendAsync(bytes, WebSocketMessageType.Binary, true, cts.Token);
    }

    public static async Task<ResponseContainer> ReceiveMessage(this ClientWebSocket client) {
        var bytes = new ArraySegment<byte>(new byte[2048]);

        WebSocketReceiveResult result;
        var i = 0;
        do {
            result = await client.ReceiveAsync(bytes[i..], CancellationToken.None);
            i += result.Count;

            if (i >= bytes.Count) {
                throw new Exception();
            }
        } while (!result.EndOfMessage);

        return MessagePackSerializer.Deserialize<ResponseContainer>(bytes[..i]);
    }
}
