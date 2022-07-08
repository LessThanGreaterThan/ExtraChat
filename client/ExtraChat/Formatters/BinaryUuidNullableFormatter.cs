using System.Buffers;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class BinaryUuidNullableFormatter : IMessagePackFormatter<Guid?> {
    public void Serialize(ref MessagePackWriter writer, Guid? value, MessagePackSerializerOptions options) {
        if (!value.HasValue) {
            writer.WriteNil();
            return;
        }

        var bytes = value.Value.ToByteArray();
        BinaryUuidFormatter.FlipBytes(bytes);
        writer.Write(bytes);
    }

    public Guid? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        var bytes = reader.ReadBytes()?.ToArray();
        if (bytes == null) {
            return null;
        }

        BinaryUuidFormatter.FlipBytes(bytes);
        return new Guid(bytes);
    }
}
