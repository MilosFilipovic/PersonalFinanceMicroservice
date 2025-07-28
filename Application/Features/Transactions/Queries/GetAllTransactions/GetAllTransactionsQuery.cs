namespace Application.Features.Transactions.Queries.GetAllTransactions;

using MediatR;
using Application.Common.Models;
using Application.DTOs;

public record GetAllTransactionsQuery(int PageNumber, int PageSize) : IRequest<PaginatedList<TransactionDto>>;

