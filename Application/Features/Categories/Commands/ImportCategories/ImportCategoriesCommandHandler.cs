
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Domain.Entities.Exceptions;

namespace Application.Features.Categories.Commands.ImportCategories;

public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, int>
{
    private readonly ICategoryCsvParser _parser;
    private readonly ICategoryRepository _repo;

    public ImportCategoriesCommandHandler(ICategoryCsvParser parser, ICategoryRepository repo)
    {
        _parser = parser;
        _repo = repo;
    }

    public async Task<int> Handle( ImportCategoriesCommand request, CancellationToken ct)
    {
        if (request.FileStream == null)
            throw new ArgumentException("Stream fajla nije prosleđen.");

        
        var records = await _parser.ParseAsync(request.FileStream, ct);

        foreach (var record in records)
        {
            if (await _repo.ExistsAsync(record.Code))
                throw new BusinessException(
                    code: "category-already-exists",
                    message: $"Kategorija sa kodom '{record.Code}' već postoji."
                );
        }

        var entities = records.Select(r => {

            return new Category
            {
                Code= r.Code,
                ParentCode= r.ParentCode,
                Name= r.Name
            };
        }).ToList();


        await _repo.AddRangeAsync(entities);
        await _repo.SaveChangesAsync();

        return entities.Count;
    }
}
