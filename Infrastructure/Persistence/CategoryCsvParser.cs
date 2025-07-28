using Application.Features.Categories.Commands.ImportCategories;
using Application.Features.Transactions.Commands.ImportTransactions;
using CsvHelper;
using CsvHelper.Configuration;
using PersonalFinanceApp.Features.Categories;
using PersonalFinanceApp.Features.Categories.Commands.ImportCategories;
using System.Globalization;

public class CategoryCsvParser : ICategoryCsvParser
{
    public Task<IEnumerable<CsvCategoryRecord>> ParseAsync(Stream file, CancellationToken ct)
    {
        using var reader = new StreamReader(file);

        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        
        using var csv = new CsvReader(reader, config);

        
        csv.Context.RegisterClassMap<CsvCategoryRecordMap>();

        
        var list = csv.GetRecords<CsvCategoryRecord>().ToList();

        return Task.FromResult<IEnumerable<CsvCategoryRecord>>(list);
    }
}
