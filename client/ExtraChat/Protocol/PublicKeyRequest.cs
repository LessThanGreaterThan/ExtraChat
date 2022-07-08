using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class PublicKeyRequest {
    [Key(0)]
    public string Name;
    
    [Key(1)]
    public ushort World;
}
