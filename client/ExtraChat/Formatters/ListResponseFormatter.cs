using ExtraChat.Protocol;
using ExtraChat.Protocol.Channels;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class ListResponseFormatter : IMessagePackFormatter<ListResponse> {
    public void Serialize(ref MessagePackWriter writer, ListResponse value, MessagePackSerializerOptions options) {
        // TODO
        throw new NotImplementedException();
    }

    public ListResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.ReadMapHeader() != 1) {
            throw new MessagePackSerializationException("Invalid map length");
        }

        var key = reader.ReadString();
        switch (key) {
            case "all": {
                if (reader.ReadArrayHeader() != 2) {
                    throw new MessagePackSerializationException("Invalid map length");
                }

                var channels = options.Resolver.GetFormatter<Channel[]>().Deserialize(ref reader, options);
                var invites = options.Resolver.GetFormatter<Channel[]>().Deserialize(ref reader, options);

                return new ListResponse.All(channels, invites);
            }
            case "channels": {
                var channels = options.Resolver.GetFormatter<SimpleChannel[]>().Deserialize(ref reader, options);

                return new ListResponse.Channels(channels);
            }
            case "members": {
                if (reader.ReadArrayHeader() != 2) {
                    throw new MessagePackSerializationException("Invalid map length");
                }

                var id = new BinaryUuidFormatter().Deserialize(ref reader, options);
                var members = options.Resolver.GetFormatter<Member[]>().Deserialize(ref reader, options);

                return new ListResponse.Members(id, members);
            }
            case "invites": {
                var channels = options.Resolver.GetFormatter<SimpleChannel[]>().Deserialize(ref reader, options);

                return new ListResponse.Invites(channels);
            }
            default: {
                throw new MessagePackSerializationException("Invalid list response type");
            }
        }
    }
}
