using ExtraChat.Formatters;
using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(MemberChangeKindFormatter))]
public abstract record MemberChangeKind {
    [MessagePackObject]
    public record Invite(string Inviter, ushort InviterWorld) : MemberChangeKind;

    [MessagePackObject]
    public record InviteDecline : MemberChangeKind;

    [MessagePackObject]
    public record InviteCancel(string Canceler, ushort CancelerWorld) : MemberChangeKind;

    [MessagePackObject]
    public record Join : MemberChangeKind;

    [MessagePackObject]
    public record Leave : MemberChangeKind;

    [MessagePackObject]
    public record Promote(Rank Rank) : MemberChangeKind;

    [MessagePackObject]
    public record Kick(string Kicker, ushort KickerWorld) : MemberChangeKind;
}
