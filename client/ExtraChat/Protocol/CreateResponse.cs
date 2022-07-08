using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class CreateResponse {
    [Key(0)]
    public Channel Channel;
}
