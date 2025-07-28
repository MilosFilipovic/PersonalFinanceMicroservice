using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
