// Application/Categories/Queries/GetAllCategoriesQuery.cs
using Application.DTOs;
using MediatR;
using System.Collections.Generic;

public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;

