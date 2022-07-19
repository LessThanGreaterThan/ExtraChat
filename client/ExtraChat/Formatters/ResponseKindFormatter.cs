using ExtraChat.Protocol;
using MessagePack;
using MessagePack.Formatters;

namespace ExtraChat.Formatters;

public class ResponseKindFormatter : IMessagePackFormatter<ResponseKind> {
    public void Serialize(ref MessagePackWriter writer, ResponseKind value, MessagePackSerializerOptions options) {
        // TODO
        throw new NotImplementedException();
    }

    public ResponseKind Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.ReadMapHeader() != 1) {
            throw new MessagePackSerializationException("Invalid ResponseKind");
        }

        var key = reader.ReadString();

        switch (key) {
            case "ping": {
                var response = options.Resolver.GetFormatterWithVerify<PingResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Ping(response);
            }
            case "error": {
                var response = options.Resolver.GetFormatterWithVerify<ErrorResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Error(response);
            }
            case "authenticate": {
                var response = options.Resolver.GetFormatterWithVerify<AuthenticateResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Authenticate(response);
            }
            case "create": {
                var response = options.Resolver.GetFormatterWithVerify<CreateResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Create(response);
            }
            case "invite": {
                var response = options.Resolver.GetFormatterWithVerify<InviteResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Invite(response);
            }
            case "invited": {
                var response = options.Resolver.GetFormatterWithVerify<InvitedResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Invited(response);
            }
            case "join": {
                var response = options.Resolver.GetFormatterWithVerify<JoinResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Join(response);
            }
            case "message": {
                var response = options.Resolver.GetFormatterWithVerify<MessageResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Message(response);
            }
            case "public_key": {
                var response = options.Resolver.GetFormatterWithVerify<PublicKeyResponse>().Deserialize(ref reader, options);
                return new ResponseKind.PublicKey(response);
            }
            case "register": {
                var response = options.Resolver.GetFormatterWithVerify<RegisterResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Register(response);
            }
            case "list": {
                var response = options.Resolver.GetFormatterWithVerify<ListResponse>().Deserialize(ref reader, options);
                return new ResponseKind.List(response);
            }
            case "leave": {
                var response = options.Resolver.GetFormatterWithVerify<LeaveResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Leave(response);
            }
            case "kick": {
                var response = options.Resolver.GetFormatterWithVerify<KickResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Kick(response);
            }
            case "disband": {
                var response = options.Resolver.GetFormatterWithVerify<DisbandResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Disband(response);
            }
            case "promote": {
                var response = options.Resolver.GetFormatterWithVerify<PromoteResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Promote(response);
            }
            case "member_change": {
                var response = options.Resolver.GetFormatterWithVerify<MemberChangeResponse>().Deserialize(ref reader, options);
                return new ResponseKind.MemberChange(response);
            }
            case "update": {
                var response = options.Resolver.GetFormatterWithVerify<UpdateResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Update(response);
            }
            case "updated": {
                var response = options.Resolver.GetFormatterWithVerify<UpdatedResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Updated(response);
            }
            case "secrets": {
                var response = options.Resolver.GetFormatterWithVerify<SecretsResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Secrets(response);
            }
            case "send_secrets": {
                var response = options.Resolver.GetFormatterWithVerify<SendSecretsResponse>().Deserialize(ref reader, options);
                return new ResponseKind.SendSecrets(response);
            }
            case "version": {
                var response = options.Resolver.GetFormatterWithVerify<VersionResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Version(response);
            }
            case "announce": {
                var response = options.Resolver.GetFormatterWithVerify<AnnounceResponse>().Deserialize(ref reader, options);
                return new ResponseKind.Announce(response);
            }
            case "delete_account": {
                var response = options.Resolver.GetFormatterWithVerify<DeleteAccountResponse>().Deserialize(ref reader, options);
                return new ResponseKind.DeleteAccount(response);
            }
            case "allow_invites": {
                var response = options.Resolver.GetFormatterWithVerify<AllowInvitesResponse>().Deserialize(ref reader, options);
                return new ResponseKind.AllowInvites(response);
            }
            default:
                throw new MessagePackSerializationException("Invalid ResponseKind");
        }
    }
}
