using System.Text;
using ExtraChat.Protocol;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class ListRequestFormatter : IMessagePackFormatter<ListRequest> {
    public void Serialize(ref MessagePackWriter writer, ListRequest value, MessagePackSerializerOptions options) {
        var plain = value switch {
            ListRequest.All => "all",
            ListRequest.Channels => "channels",
            ListRequest.Invites => "invites",
            _ => null,
        };

        if (plain != null) {
            writer.WriteString(Encoding.UTF8.GetBytes(plain));
            return;
        }

        writer.WriteMapHeader(1);

        switch (value) {
            case ListRequest.Members members: {
                writer.WriteString(Encoding.UTF8.GetBytes("members"));
                new BinaryUuidFormatter().Serialize(ref writer, members.ChannelId, options);

                break;
            }
            default: {
                throw new MessagePackSerializationException("Invalid ListRequest value");
            }
        }
    }

    public ListRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        // TODO
        throw new NotImplementedException();
    }
}
