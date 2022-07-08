using MessagePack;

namespace ExtraChat.Protocol.Channels;

[Serializable]
[MessagePackObject]
public class Member {
    [Key(0)]
    public string Name;

    [Key(1)]
    public ushort World;

    [Key(2)]
    public Rank Rank;

    [Key(3)]
    public bool Online;
}
