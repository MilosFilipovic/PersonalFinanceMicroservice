using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Features.Transactions.Commands.SplitTransaction
{
    public class SplitTransactionCommandHandler
        : IRequestHandler<SplitTransactionCommand>
    {
        private readonly ITransactionRepository _txRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly ITransactionSplitRepository _splitRepo;
        private readonly IUnitOfWork _uow;

        public SplitTransactionCommandHandler(
            ITransactionRepository txRepo,
            ICategoryRepository catRepo,
            ITransactionSplitRepository splitRepo,
            IUnitOfWork uow)
        {
            _txRepo = txRepo;
            _catRepo = catRepo;
            _splitRepo = splitRepo;
            _uow = uow;
        }

        public async Task<Unit> Handle(SplitTransactionCommand cmd, CancellationToken ct)
        {
            
            var tx = await _txRepo.GetByIdAsync(cmd.TransactionId)
                     ?? throw new KeyNotFoundException($"Transaction {cmd.TransactionId} not found.");

            
            var allCats = await _catRepo.GetAllAsync(ct);
            var invalid = cmd.Splits
                .Select(s => s.CatCode)
                .Except(allCats.Select(c => c.Code), StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (invalid.Any())
                throw new ValidationException($"Categories not found: {string.Join(',', invalid)}");

            
            await _splitRepo.DeleteByTransactionIdAsync(cmd.TransactionId, ct);

            
            var newSplits = cmd.Splits.Select(s => new Domain.Entities.TransactionSplit
            {
                
                TransactionId = cmd.TransactionId,
                CatCode = s.CatCode,
                Amount = s.Amount
            }).ToList();

            await _splitRepo.AddRangeAsync(newSplits, ct);

            
            await _uow.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
