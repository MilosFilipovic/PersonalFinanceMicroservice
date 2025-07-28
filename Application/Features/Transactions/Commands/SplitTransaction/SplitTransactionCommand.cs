using Application.DTOs;
using MediatR;

namespace Application.Features.Transactions.Commands.SplitTransaction
{
    public record SplitTransactionCommand(
        string TransactionId,
        List<SplitItemDto> Splits
    ) : IRequest;
}
