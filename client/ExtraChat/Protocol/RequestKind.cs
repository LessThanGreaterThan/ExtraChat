using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(RequestKindFormatter))]
public abstract record RequestKind {
    [MessagePackObject]
    public record Ping(PingRequest Request) : RequestKind;

    [MessagePackObject]
    public record Register(RegisterRequest Request) : RequestKind;

    [MessagePackObject]
    public record Authenticate(AuthenticateRequest Request) : RequestKind;

    [MessagePackObject]
    public record Message(MessageRequest Request) : RequestKind;

    [MessagePackObject]
    public record Create(CreateRequest Request) : RequestKind;

    [MessagePackObject]
    public record PublicKey(PublicKeyRequest Request) : RequestKind;

    [MessagePackObject]
    public record Invite(InviteRequest Request) : RequestKind;

    [MessagePackObject]
    public record Join(JoinRequest Request) : RequestKind;

    [MessagePackObject]
    public record List(ListRequest Request) : RequestKind;

    [MessagePackObject]
    public record Leave(LeaveRequest Request) : RequestKind;

    [MessagePackObject]
    public record Kick(KickRequest Request) : RequestKind;

    [MessagePackObject]
    public record Disband(DisbandRequest Request) : RequestKind;

    [MessagePackObject]
    public record Promote(PromoteRequest Request) : RequestKind;

    [MessagePackObject]
    public record Update(UpdateRequest Request) : RequestKind;

    [MessagePackObject]
    public record Secrets(SecretsRequest Request) : RequestKind;

    [MessagePackObject]
    public record SendSecrets(SendSecretsRequest Request) : RequestKind;
}
