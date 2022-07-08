using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class MessageResponse {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;
    
    [Key(1)]
    public string Sender;
    
    [Key(2)]
    public ushort World;
    
    [Key(3)]
    public byte[] Message;
}
