using MessagePack;

namespace ExtraChat.Protocol;

[Serializable]
[MessagePackObject]
public class VersionResponse {
    [Key(0)]
    public uint Version;
}
