namespace Application.Features.Transactions.Commands.ImportTransactions
{
    public class CsvTransactionRecord
    {
        public string Id { get; set; } = null!;
        public string BeneficiaryName { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Direction { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public int? Mcc { get; set; }
        public string Kind { get; set; } = null!;
        public string? CatCode { get; set; }
    }

}
