using ExtraChat.Protocol.Channels;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class InvitedResponse {
    [Key(0)]
    public Channel Channel;

    [Key(1)]
    public string Name;

    [Key(2)]
    public ushort World;

    [Key(3)]
    public byte[] PublicKey;

    [Key(4)]
    public byte[] EncryptedSecret;
}
