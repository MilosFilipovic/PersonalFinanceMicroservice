using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class TransactionSplitRepository : ITransactionSplitRepository
    {
        private readonly AppDbContext _context;
        public TransactionSplitRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<TransactionSplit>> GetByTransactionIdAsync(string txId, CancellationToken ct = default)
            => await _context.TransactionSplits
                     .Where(s => s.TransactionId == txId)
                     .AsNoTracking()
                     .ToListAsync(ct);

        public async Task DeleteByTransactionIdAsync(string txId, CancellationToken ct = default)
        {
            var existing = await _context.TransactionSplits
                                .Where(s => s.TransactionId == txId)
                                .ToListAsync(ct);
            _context.TransactionSplits.RemoveRange(existing);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddRangeAsync(IEnumerable<TransactionSplit> splits, CancellationToken ct = default)
        {
            await _context.TransactionSplits.AddRangeAsync(splits, ct);
            await _context.SaveChangesAsync(ct);
        }

        
    }
}
