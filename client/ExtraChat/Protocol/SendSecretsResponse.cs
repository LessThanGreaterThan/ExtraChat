using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class SendSecretsResponse {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;

    [Key(1)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid RequestId;

    [Key(2)]
    public byte[] PublicKey;
}
