using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Entities.Enums
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionKind
    {
        [EnumMember(Value = "dep")] Dep,
        [EnumMember(Value = "wdw")] Wdw,
        [EnumMember(Value = "pmt")] Pmt,
        [EnumMember(Value = "fee")] Fee,
        [EnumMember(Value = "inc")] Inc,
        [EnumMember(Value = "rev")] Rev,
        [EnumMember(Value = "adj")] Adj,
        [EnumMember(Value = "lnd")] Lnd,
        [EnumMember(Value = "lnr")] Lnr,
        [EnumMember(Value = "fcx")] Fcx,
        [EnumMember(Value = "aop")] Aop,
        [EnumMember(Value = "acl")] Acl,
        [EnumMember(Value = "spl")] Spl,
        [EnumMember(Value = "sal")] Sal
    }
}