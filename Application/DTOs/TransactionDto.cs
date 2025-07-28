using System;
using System.Text.Json.Serialization;
using Domain.Entities.Enums;

namespace Application.DTOs;

public class TransactionDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!; 

    [JsonPropertyName("beneficiary-name")]
    public required string BeneficiaryName { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("direction")]
    public Direction Direction { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("currency")]
    public required string Currency { get; set; }

    [JsonPropertyName("mcc")]
    public Mcc? Mcc { get; set; }

    [JsonPropertyName("kind")]
    public TransactionKind Kind { get; set; }

    [JsonPropertyName("category-code")]
    public string? CatCode { get; set; }

    public List<SplitItemDto>? Splits { get; set; }
}


