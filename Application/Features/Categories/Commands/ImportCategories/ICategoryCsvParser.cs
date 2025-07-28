using Microsoft.AspNetCore.Http;
using PersonalFinanceApp.Features.Categories.Commands.ImportCategories;

public interface ICategoryCsvParser
{
    Task<IEnumerable<CsvCategoryRecord>> ParseAsync(Stream file, CancellationToken ct);
}
