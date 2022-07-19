using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(ResponseKindFormatter))]
public abstract record ResponseKind {
    [MessagePackObject]
    public record Ping(PingResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Error(ErrorResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Register(RegisterResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Authenticate(AuthenticateResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Message(MessageResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Create(CreateResponse Response) : ResponseKind;

    [MessagePackObject]
    public record PublicKey(PublicKeyResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Invite(InviteResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Invited(InvitedResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Join(JoinResponse Response) : ResponseKind;

    [MessagePackObject]
    public record List(ListResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Leave(LeaveResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Kick(KickResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Disband(DisbandResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Promote(PromoteResponse Response) : ResponseKind;

    [MessagePackObject]
    public record MemberChange(MemberChangeResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Update(UpdateResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Updated(UpdatedResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Secrets(SecretsResponse Response) : ResponseKind;

    [MessagePackObject]
    public record SendSecrets(SendSecretsResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Version(VersionResponse Response) : ResponseKind;

    [MessagePackObject]
    public record Announce(AnnounceResponse Response) : ResponseKind;

    [MessagePackObject]
    public record DeleteAccount(DeleteAccountResponse Response) : ResponseKind;

    [MessagePackObject]
    public record AllowInvites(AllowInvitesResponse Response) : ResponseKind;
}
