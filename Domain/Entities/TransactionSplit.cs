using Domain.Models;

namespace Domain.Entities
{
    public class TransactionSplit
    {
        public int Id { get; set; }
        public required string TransactionId { get; set; }
        public string CatCode { get; set; } = default!;
        public decimal Amount { get; set; }

        
        public Transaction? Transaction { get; set; }
        public Category? Category { get; set; }
    }
}
