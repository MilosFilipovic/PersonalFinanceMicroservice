using MediatR;

public class CategorizeTransactionCommand : IRequest<Unit>
{
    public required string TransactionId { get; set; }
    public required string CategoryCode { get; set; }
}
