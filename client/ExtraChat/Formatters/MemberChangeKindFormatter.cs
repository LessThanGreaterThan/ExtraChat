using ExtraChat.Protocol;
using ExtraChat.Protocol.Channels;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class MemberChangeKindFormatter : IMessagePackFormatter<MemberChangeKind> {
    public void Serialize(ref MessagePackWriter writer, MemberChangeKind value, MessagePackSerializerOptions options) {
        throw new NotImplementedException();
    }

    public MemberChangeKind Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.NextMessagePackType == MessagePackType.String) {
            return reader.ReadString() switch {
                "join" => new MemberChangeKind.Join(),
                "leave" => new MemberChangeKind.Leave(),
                "invite_decline" => new MemberChangeKind.InviteDecline(),
                _ => throw new MessagePackSerializationException("invalid MemberChangeKind key"),
            };
        }

        if (reader.ReadMapHeader() != 1) {
            throw new MessagePackSerializationException("Invalid map length");
        }

        var key = reader.ReadString();
        switch (key) {
            case "invite": {
                if (reader.ReadArrayHeader() != 2) {
                    throw new MessagePackSerializationException("Invalid array length");
                }

                var inviter = reader.ReadString();
                var world = reader.ReadUInt16();
                return new MemberChangeKind.Invite(inviter, world);
            }
            case "invite_cancel": {
                if (reader.ReadArrayHeader() != 2) {
                    throw new MessagePackSerializationException("Invalid array length");
                }

                var canceler = reader.ReadString();
                var world = reader.ReadUInt16();
                return new MemberChangeKind.InviteCancel(canceler, world);
            }
            case "promote": {
                if (reader.ReadArrayHeader() != 1) {
                    throw new MessagePackSerializationException("Invalid array length");
                }

                var rank = options.Resolver.GetFormatter<Rank>().Deserialize(ref reader, options);
                return new MemberChangeKind.Promote(rank);
            }
            case "kick": {
                if (reader.ReadArrayHeader() != 2) {
                    throw new MessagePackSerializationException("Invalid array length");
                }

                var kicker = reader.ReadString();
                var world = reader.ReadUInt16();
                return new MemberChangeKind.Kick(kicker, world);
            }
            default: {
                throw new MessagePackSerializationException("invalid MemberChangeKind key");
            }
        }
    }
}
