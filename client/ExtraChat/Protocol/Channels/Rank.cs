namespace ExtraChat.Protocol.Channels;

[Serializable]
public enum Rank : byte {
    Invited = 0,
    Member = 1,
    Moderator = 2,
    Admin = 3,
}

internal static class RankExt {
    internal static string Symbol(this Rank rank) => rank switch {
        // invited: a question mark with a circle around it
        Rank.Invited => "? ",
        Rank.Member => "",
        Rank.Moderator => "☆ ",
        Rank.Admin => "★ ",
        _ => "",
    };
}
