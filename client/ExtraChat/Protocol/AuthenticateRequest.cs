using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class AuthenticateRequest {
    [Key(0)]
    public string Key;

    [Key(1)]
    public byte[] PublicKey;
}
