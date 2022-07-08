using System.Buffers;
using System.Text;
using ExtraChat.Protocol;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class UpdateKindFormatter : IMessagePackFormatter<UpdateKind> {
    public void Serialize(ref MessagePackWriter writer, UpdateKind value, MessagePackSerializerOptions options) {
        writer.WriteMapHeader(1);

        var key = value switch {
            UpdateKind.Name => "name",
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };

        writer.WriteString(Encoding.UTF8.GetBytes(key));

        switch (value) {
            case UpdateKind.Name name: {
                writer.Write(name.NewName);
                break;
            }
            default: {
                throw new MessagePackSerializationException("Unknown UpdateKind");
            }
        }
    }

    public UpdateKind Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.ReadMapHeader() != 1) {
            throw new MessagePackSerializationException("UpdateKindFormatter: Invalid map length");
        }

        var key = reader.ReadString();
        switch (key) {
            case "name": {
                var name = reader.ReadBytes()!.Value.ToArray();
                return new UpdateKind.Name(name);
            }
            default: {
                throw new MessagePackSerializationException("UpdateKindFormatter: Invalid key");
            }
        }
    }
}
