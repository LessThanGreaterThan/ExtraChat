using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class AuthenticateResponse {
    [Key(0)]
    public string? Error;
}
