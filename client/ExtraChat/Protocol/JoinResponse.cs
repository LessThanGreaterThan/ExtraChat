using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class JoinResponse {
    [Key(0)]
    public Channel Channel;
}
