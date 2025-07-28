using System.Globalization;
using Application.Features.Transactions.Commands.ImportTransactions; // CsvTransactionRecord
using CsvHelper;
using CsvHelper.Configuration;


public class TransactionCsvParser : ITransactionCsvParser
{
    public Task<IEnumerable<CsvTransactionRecord>> ParseAsync(Stream csvStream, CancellationToken ct = default)
    {
        using var reader = new StreamReader(csvStream);

        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,       
            HeaderValidated = null,       
            PrepareHeaderForMatch = args => args.Header
                                             .Replace("-", "")
                                             .ToLower()   
        };

        
        using var csv = new CsvReader(reader, config);

       
        csv.Context.RegisterClassMap<CsvTransactionRecordMap>();

        
        var list = csv.GetRecords<CsvTransactionRecord>().ToList();

        return Task.FromResult<IEnumerable<CsvTransactionRecord>>(list);
    }
}
