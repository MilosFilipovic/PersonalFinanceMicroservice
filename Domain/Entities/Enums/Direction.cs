using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Entities.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Direction
    {
        [EnumMember(Value = "d")]
        Debit,
        [EnumMember(Value = "c")]
        Credit
    }
}
