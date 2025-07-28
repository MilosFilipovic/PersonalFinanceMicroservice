using System.ComponentModel.DataAnnotations;

public class CategorizeDto
{
    [Required]
    public required string CategoryCode { get; set; }
}
