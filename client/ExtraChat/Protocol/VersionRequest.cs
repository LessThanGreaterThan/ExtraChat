using MessagePack;

namespace ExtraChat.Protocol; 

[Serializable]
[MessagePackObject]
public class VersionRequest {
    [Key(0)]
    public uint Version;
}
