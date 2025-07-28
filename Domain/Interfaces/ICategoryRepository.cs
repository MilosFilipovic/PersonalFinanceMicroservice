using Domain.Models;

namespace Domain.Interfaces;
public interface ICategoryRepository
{

    Task<Category> GetByCodeAsync(string code);
    Task AddRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    
    Task<bool> ExistsAsync(string code);

    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
}