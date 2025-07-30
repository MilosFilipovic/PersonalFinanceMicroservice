namespace PersonalFinanceApp.Features.Categories.Commands.ImportCategories
{
    public class CsvCategoryRecord
    {
        
        public string Code { get; set; } = null!;

        
        public string Name { get; set; } = null!;

        
        public string? ParentCode { get; set; }
    }
}
