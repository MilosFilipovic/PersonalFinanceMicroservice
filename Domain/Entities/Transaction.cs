using System;

namespace Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public string BeneficiaryName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Direction { get; set; } = string.Empty; // npr. "debit" ili "credit"

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Currency { get; set; } = "RSD";

        public int? Mcc { get; set; } // Merchant Category Code – može biti null

        public string Kind { get; set; } = string.Empty; // npr. "expense", "income", "transfer"
    }
}

