using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class PublicKeyResponse {
    [Key(0)]
    public string Name;
    
    [Key(1)]
    public ushort World;
    
    [Key(2)]
    public byte[]? PublicKey;
}
