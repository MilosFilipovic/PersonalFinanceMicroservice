using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITransactionSplitRepository
    {
        Task<IEnumerable<TransactionSplit>> GetByTransactionIdAsync(string txId, CancellationToken ct = default);
        Task DeleteByTransactionIdAsync(string txId, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<TransactionSplit> splits, CancellationToken ct = default);
    }
}
