namespace Application.Features.Transactions.Commands.CreateTransaction;

using Domain.Interfaces;
using Domain.Entities;
using Domain.Entities.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, string>
{
    private readonly ITransactionRepository _repository;

    public CreateTransactionCommandHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    
    public async Task<string> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var direction = Enum.Parse<Direction>(request.direction, ignoreCase: true);
        var kind = Enum.Parse<TransactionKind>(request.kind, ignoreCase: true);


        var entity = new Transaction
        {
            Id = request.id,
            BeneficiaryName = request.beneficiaryName,
            Date = request.Date,
            Direction = direction,
            Amount = request.Amount,
            Description = request.Description,
            Currency = request.currency,
            Mcc = (Mcc?)request.mcc,
            Kind = kind
        };
        var created = await _repository.AddAsync(entity, cancellationToken);
        
        return created.Id;
    }
    
}
