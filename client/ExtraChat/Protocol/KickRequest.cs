using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol; 

[Serializable]
[MessagePackObject]
public class KickRequest {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;
    
    [Key(1)]
    public string Name;
    
    [Key(2)]
    public ushort World;
}
