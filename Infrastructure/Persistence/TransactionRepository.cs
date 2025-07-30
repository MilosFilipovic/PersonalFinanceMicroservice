namespace Infrastructure.Persistence
{
    using Domain.Entities;
    using Domain.Entities.Enums;
    using Domain.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;


    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddAsync(Transaction entity, CancellationToken cancellationToken)
        {
            var entry = await _context.Transactions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }



        public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Transactions
                                 .AsNoTracking()
                                 .ToListAsync(cancellationToken);
        }


        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int pageNumber,
            int pageSize,
            List<TransactionKind>? kinds = null,
            string? sortBy = null,
            string sortOrder = "asc",
            CancellationToken ct = default)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1);

            IQueryable<Transaction> query = _context.Transactions
                .Include(t => t.Splits)
                .Where(t => t.Date >= start && t.Date <= end);

            if (kinds?.Any() == true)
                query = query.Where(t => kinds.Contains(t.Kind));

            var desc = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);// SORT
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.Trim().ToLower())
                {
                    case "date":
                        query = desc
                            ? query.OrderByDescending(t => t.Date)
                            : query.OrderBy(t => t.Date);
                        break;
                    case "amount":
                        query = desc
                            ? query.OrderByDescending(t => t.Amount)
                            : query.OrderBy(t => t.Amount);
                        break;
                    case "beneficiaryname":
                    case "beneficiary-name":
                        query = desc
                            ? query.OrderByDescending(t => t.BeneficiaryName)
                            : query.OrderBy(t => t.BeneficiaryName);
                        break;
                    
                    default:
                        query = query.OrderByDescending(t => t.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(t => t.Date);
            }

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }



        //public async Task<Transaction> GetByIdAsync(string id)
        //{
        //    return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        //}

        public async Task<Transaction> GetByIdAsync(string id)
        {
            return await _context.Transactions
                                 .FirstAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .AnyAsync(t => t.Id == id, ct);
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Transactions.AsNoTracking().OrderByDescending(t=>t.Date);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<IEnumerable<Transaction>> GetForAnalyticsAsync( string? categoryCode, DateTime? startDate, DateTime? endDate,
                                                                                Direction? direction, CancellationToken ct = default)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoryCode))
                query = query.Where(t => t.CatCode == categoryCode || t.Category.ParentCode == categoryCode);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if (direction.HasValue)
                query = query.Where(t => t.Direction == direction.Value);

            return await query.ToListAsync(ct);
        }


        public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
        {

            await _context.Transactions.AddRangeAsync(transactions, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }



        public Task<List<Transaction>> GetUncategorizedAsync()=> _context.Transactions.Where(t => t.CatCode == null).ToListAsync();
    }
}





