using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol.Channels; 

[Serializable]
[MessagePackObject]
public class SimpleChannel {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Id;
    
    [Key(1)]
    public byte[] Name;

    [Key(2)]
    public Rank Rank;
}
