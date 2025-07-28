namespace Application.Common.Interfaces;

using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction entity);
    Task<IEnumerable<Transaction>> GetAllAsync();
}

