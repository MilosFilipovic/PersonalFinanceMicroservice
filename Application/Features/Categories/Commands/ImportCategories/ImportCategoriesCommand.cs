
using MediatR;

public class ImportCategoriesCommand : IRequest<int>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
