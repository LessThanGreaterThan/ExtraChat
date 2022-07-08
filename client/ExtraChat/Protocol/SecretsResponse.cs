using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class SecretsResponse {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;

    [Key(1)]
    public byte[] PublicKey;

    [Key(2)]
    public byte[] EncryptedSharedSecret;
}
