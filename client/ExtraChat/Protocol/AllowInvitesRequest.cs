using MessagePack;

namespace ExtraChat.Protocol; 

[Serializable]
[MessagePackObject]
public class AllowInvitesRequest {
    [Key(0)]
    public bool Allowed;
}

[Serializable]
[MessagePackObject]
public class AllowInvitesResponse {
    [Key(0)]
    public bool Allowed;
}
