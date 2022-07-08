using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class RegisterRequest {
    [Key(0)]
    public string Name;

    [Key(1)]
    public ushort World;

    [Key(2)]
    public bool ChallengeCompleted;
}
