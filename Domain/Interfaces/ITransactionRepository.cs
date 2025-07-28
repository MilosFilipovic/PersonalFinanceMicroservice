namespace Domain.Interfaces
{
    using Domain.Entities;
    using Domain.Entities.Enums;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction entity, CancellationToken cancellationToken);
        Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken);

        
        Task<(IEnumerable<Transaction> Items, int TotalCount)>GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Transaction> GetByIdAsync(string id);

        Task<bool> ExistsAsync(string id, CancellationToken ct = default);

        Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<(IEnumerable<Transaction> Items, int TotalCount)>GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, 
                                                                    int pageSize, List<TransactionKind>? kinds = null, CancellationToken ct = default);

        Task<IEnumerable<Transaction>> GetForAnalyticsAsync( string? categoryCode, DateTime? startDate, DateTime? endDate, Direction? direction, CancellationToken ct = default);

        Task<List<Transaction>> GetUncategorizedAsync();
    }
}

