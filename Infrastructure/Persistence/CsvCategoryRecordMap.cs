using CsvHelper.Configuration;
using PersonalFinanceApp.Features.Categories.Commands.ImportCategories;

public sealed class CsvCategoryRecordMap : ClassMap<CsvCategoryRecord>
{
    public CsvCategoryRecordMap()
    {
        Map(m => m.Code).Name("code");
        Map(m => m.ParentCode).Name("parent-code");
        Map(m => m.Name).Name("name");
        


    }
}