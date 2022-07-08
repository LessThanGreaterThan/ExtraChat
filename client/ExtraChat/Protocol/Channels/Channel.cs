using System.Text;
using ExtraChat.Formatters;
using ExtraChat.Util;
using MessagePack;

namespace ExtraChat.Protocol.Channels;

[Serializable]
[MessagePackObject]
public class Channel {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidFormatter))]
    public Guid Id;

    [Key(1)]
    public byte[] Name;

    [Key(2)]
    public List<Member> Members;

    internal string DecryptName(byte[] key) {
        return Encoding.UTF8.GetString(SecretBox.Decrypt(key, this.Name));
    }
}
