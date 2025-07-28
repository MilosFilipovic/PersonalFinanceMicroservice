using System.Text.Json.Serialization;
using Domain.Entities.Enums;


namespace Application.DTOs;
public class CategoryDto
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("parent-code")]
    public string? ParentCode { get; set; }
}
