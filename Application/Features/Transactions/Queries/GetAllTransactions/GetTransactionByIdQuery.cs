namespace Application.Features.Transactions.Queries.GetAllTransactions;

using MediatR;
using Application.DTOs;

public record GetTransactionByIdQuery(string id) : IRequest<TransactionDto>;