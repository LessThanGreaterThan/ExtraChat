using MessagePack;

namespace ExtraChat.Protocol;

[MessagePackObject]
public class AnnounceResponse {
    [Key(0)]
    public string Announcement;
}
