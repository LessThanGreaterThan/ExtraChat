using System.Buffers;
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
        const int maxSize = 1_048_576;
        // rent a 1 MiB byte array from the shared runtime pool
        var bytes = ArrayPool<byte>.Shared.Rent(maxSize);

        try {
            WebSocketReceiveResult result;
            var i = 0;
            do {
                result = await client.ReceiveAsync(bytes[i..maxSize], CancellationToken.None);
                i += result.Count;

                // break if we've filled up the buffer, even if message isn't complete
                if (i == maxSize) {
                    break;
                }

                if (i > maxSize) {
                    throw new Exception("read too many bytes for one message");
                }
            } while (!result.EndOfMessage);

            return MessagePackSerializer.Deserialize<ResponseContainer>(bytes);
        } finally {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
