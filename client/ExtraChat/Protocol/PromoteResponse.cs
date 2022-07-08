using ExtraChat.Formatters;
using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class PromoteResponse {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;

    [Key(1)]
    public string Name;

    [Key(2)]
    public ushort World;

    [Key(3)]
    public Rank Rank;
}
