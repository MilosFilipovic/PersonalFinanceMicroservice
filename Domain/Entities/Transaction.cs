using Domain.Entities.Enums;
using Domain.Models;

namespace  Domain.Entities
{
    public class Transaction
    {
        public string Id { get; set; } = null!;
        public DateTime Date { get; set; } 
        public Direction Direction { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TransactionKind Kind { get; set; }

        public Mcc? Mcc { get; set; }

        public string BeneficiaryName { get; set; }
        public string Description { get; set; }

        public string? CatCode { get; set; }





        public Category? Category { get; set; }


        public List<TransactionSplit> Splits { get; set; } = new();




        public Transaction() { }

        
        public Transaction(
            string id,
            DateTime date,
            Direction direction,
            decimal amount,
            string currency,
            TransactionKind kind,
            int? mcc,
            string beneficiaryName,
            string description
            
            )
        {
            
            if (amount <= 0) throw new ArgumentException("Amount mora biti > 0.");
            if (currency?.Length != 3) throw new ArgumentException("Currency mora biti troslovna.");
            

            Id = id;
            Date = date;
            Direction = direction;
            Amount = amount;
            Currency = currency.ToUpperInvariant();
            Kind = kind;
            Mcc = (Mcc?)mcc;
            BeneficiaryName = beneficiaryName;
            Description = description;
        }

        public void AssignCategory(string? catCode)
        {
            CatCode = catCode;
        }

        public void Categorize(Category category)
        {
            
            this.CatCode = category.Code;
            this.Category = category;
        }


        

        
    }

}