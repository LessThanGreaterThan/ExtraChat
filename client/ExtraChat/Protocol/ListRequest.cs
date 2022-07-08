using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(ListRequestFormatter))]
public abstract record ListRequest {
    public record All : ListRequest;

    public record Channels : ListRequest;

    public record Members(Guid ChannelId) : ListRequest;

    public record Invites : ListRequest;
}
