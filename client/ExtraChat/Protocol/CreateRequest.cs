using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class CreateRequest {
    [Key(0)]
    public byte[] Name;
}
