using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class RequestContainer {
    [Key(0)]
    public uint Number;

    [Key(1)]
    public RequestKind Kind;
}
