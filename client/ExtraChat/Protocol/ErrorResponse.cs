using ExtraChat.Formatters;
using MessagePack;

namespace ExtraChat.Protocol; 

[Serializable]
[MessagePackObject]
public class ErrorResponse {
    [Key(0)]
    [MessagePackFormatter(typeof(BinaryUuidNullableFormatter))]
    public Guid? Channel;
    
    [Key(1)]
    public string Error;
}
