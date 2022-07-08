using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class ResponseContainer {
    [Key(0)]
    public uint Number;

    [Key(1)]
    public ResponseKind Kind;
}
