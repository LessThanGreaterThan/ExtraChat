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
        var bytes = new List<byte>(2048);
        var buffer = new ArraySegment<byte>(new byte[2048]);

        WebSocketReceiveResult result;
        do {
            result = await client.ReceiveAsync(buffer, CancellationToken.None);
            bytes.AddRange(buffer[..result.Count]);

            // 1 MiB
            if (bytes.Count > 1_048_576) {
                throw new Exception();
            }
        } while (!result.EndOfMessage);

        return MessagePackSerializer.Deserialize<ResponseContainer>(bytes.ToArray());
    }
}
