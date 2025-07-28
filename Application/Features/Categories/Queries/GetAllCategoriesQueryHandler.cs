using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

public class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repo.GetAllAsync(cancellationToken);
        // sada AutoMapper zna mapu Category → CategoryDto
        return _mapper.Map<IEnumerable<CategoryDto>>(entities);
    }
}
