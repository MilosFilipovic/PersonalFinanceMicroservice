using FluentValidation.Results;
using Domain.Interfaces;
using MediatR;
using Domain.Entities.Exceptions;
using FluentValidation;

public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CategorizeTransactionCommandHandler(ITransactionRepository transactionRepo, ICategoryRepository categoryRepo, IUnitOfWork unitOfWork)
    {
        _transactionRepo = transactionRepo;
        _categoryRepo = categoryRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CategorizeTransactionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TransactionId))
            throw new ValidationException(new[]
            {
                new ValidationFailure("id", "Parametar 'id' je obavezan.")
            });

        
        var txn = await _transactionRepo.GetByIdAsync(request.TransactionId);
        if (txn == null)
            throw new ValidationException(new[]
            {
                new ValidationFailure("id", $"Transakcija '{request.TransactionId}' ne postoji.")
            });

        
        var exists = await _categoryRepo.ExistsAsync(request.CategoryCode);
        if (!exists)
            throw new BusinessException(
                code: "provided-category-does-not-exists",
                message: $"Kategorija '{request.CategoryCode}' ne postoji."
            );

        
        var cat = await _categoryRepo.GetByCodeAsync(request.CategoryCode);

        
        txn.Categorize(cat);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
