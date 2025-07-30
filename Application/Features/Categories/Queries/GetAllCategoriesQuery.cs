
using Application.DTOs;
using MediatR;


public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;

