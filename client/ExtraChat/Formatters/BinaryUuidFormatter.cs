using System.Buffers;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters; 

public class BinaryUuidFormatter : IMessagePackFormatter<Guid> {
    public void Serialize(ref MessagePackWriter writer, Guid value, MessagePackSerializerOptions options) {
        var bytes = value.ToByteArray();
        FlipBytes(bytes);
        writer.Write(bytes);
    }

    public Guid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        var bytes = reader.ReadBytes()!.Value.ToArray();
        FlipBytes(bytes);
        return new Guid(bytes);
    }

    internal static void FlipBytes(byte[] bytes) {
        // microsoft is stupid for no reason
        Array.Reverse(bytes,0,4);
        Array.Reverse(bytes,4,2);
        Array.Reverse(bytes,6,2);
    }
}
