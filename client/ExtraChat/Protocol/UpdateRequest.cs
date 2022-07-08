using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class UpdateRequest {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Channel;

    [Key(1)]
    public UpdateKind Kind;
}
