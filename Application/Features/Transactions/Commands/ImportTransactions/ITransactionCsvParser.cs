using Application.Features.Transactions.Commands.ImportTransactions;

public interface ITransactionCsvParser
{
    Task<IEnumerable<CsvTransactionRecord>> ParseAsync(Stream csvStream, CancellationToken ct = default);
}
