using MediatR;


namespace Application.Features.Transactions.Commands.ImportTransactions;

public class ImportTransactionsCommand : IRequest<int>
{
    public Stream? FileStream { get; set; }   
    public string? FileName { get; set; }    
}
