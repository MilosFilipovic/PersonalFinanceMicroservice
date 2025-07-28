using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext db) => _context = db;

    //public async Task<Category> GetByCodeAsync(string code)
    //{
    //    return await _context.Categories.FirstOrDefaultAsync(c => c.Code == code);
    //}

    public async Task<Category> GetByCodeAsync(string code)
    {
        return await _context.Categories
                             .FirstAsync(c => c.Code == code);
    }

    public async Task<bool> ExistsAsync(string code) =>
        await _context.Categories.AnyAsync(c => c.Code == code);



    public async Task AddRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddRangeAsync(categories, cancellationToken);
    }



    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task<List<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync(ct);
    }

}   