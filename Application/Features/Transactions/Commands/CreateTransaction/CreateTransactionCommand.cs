namespace Application.Features.Transactions.Commands.CreateTransaction;

using MediatR;
using System;

public record CreateTransactionCommand(string id, string beneficiaryName, DateTime Date, string direction, 
                                       decimal Amount, string Description, string currency, int? mcc, string kind) : IRequest<string>;


