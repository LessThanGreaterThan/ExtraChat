using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
[MessagePackFormatter(typeof(UpdateKindFormatter))]
public abstract record UpdateKind {
    [MessagePackObject]
    public record Name(byte[] NewName) : UpdateKind;
}
