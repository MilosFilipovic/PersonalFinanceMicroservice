using Application.Common.Models;
using Application.DTOs;
using Domain.Entities.Enums;
using MediatR;

public class GetTransactionsByDateRangeQuery : IRequest<PaginatedList<TransactionDto>>
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public List<TransactionKind>? Kinds { get; init; }

    public string? SortBy { get; init; }

    public string SortOrder { get; init; } = "asc";
}
