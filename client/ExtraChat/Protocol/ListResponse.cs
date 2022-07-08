using ExtraChat.Formatters;
using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(ListResponseFormatter))]
public abstract record ListResponse {
    [MessagePackObject]
    public record All(Channel[] AllChannels, Channel[] AllInvites) : ListResponse;

    [MessagePackObject]
    public record Channels(SimpleChannel[] SimpleChannels) : ListResponse;

    [MessagePackObject]
    public record Members(Guid ChannelId, Member[] AllMembers) : ListResponse;

    [MessagePackObject]
    public record Invites(SimpleChannel[] AllInvites) : ListResponse;
}
