using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class JoinRequest {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;
}
