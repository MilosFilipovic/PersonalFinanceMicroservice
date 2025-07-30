using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Transactions.Commands.ImportTransactions;

public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, int>
{
    //private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    //{
    //    Converters = { new JsonStringEnumConverter() }
    //};

    private readonly ITransactionRepository _repo;
    private readonly ITransactionCsvParser _parser;

    public ImportTransactionsCommandHandler(ITransactionRepository repo, ITransactionCsvParser parser)
    {
        _repo = repo;
        _parser = parser;
    }

    public async Task<int> Handle(ImportTransactionsCommand request, CancellationToken ct)
    {
        if (request.FileStream == null)
            throw new ArgumentException("Stream fajla nije prosleđen.");

        var records = await _parser.ParseAsync(request.FileStream, ct);

        var entities = records.Select(r => {
            var direction = r.Direction switch
                {
                    "d" => Direction.Debit,
                    "c" => Direction.Credit,
                    _ => throw new ArgumentOutOfRangeException(
                                nameof(r.Direction),
                                r.Direction,
                                "Nepoznata vrednost za Direction")
                };

                var kind = r.Kind switch
                {
                    "dep" => TransactionKind.Dep,
                    "wdw" => TransactionKind.Wdw,
                    "pmt" => TransactionKind.Pmt,
                    "fee" => TransactionKind.Fee,
                    "inc" => TransactionKind.Inc,
                    "rev" => TransactionKind.Rev,
                    "adj" => TransactionKind.Adj,
                    "lnd" => TransactionKind.Lnd,
                    "lnr" => TransactionKind.Lnr,
                    "fcx" => TransactionKind.Fcx,
                    "aop" => TransactionKind.Aop,
                    "acl" => TransactionKind.Acl,
                    "spl" => TransactionKind.Spl,
                    "sal" => TransactionKind.Sal,
                    _ => throw new ArgumentOutOfRangeException(
                                  nameof(r.Kind),
                                  r.Kind,
                                  "Nepoznata vrednost za TransactionKind")
                };

            var tx = new Transaction
                  {
                    Id = r.Id,
                    BeneficiaryName = r.BeneficiaryName,
                    Date = DateTime.SpecifyKind(r.Date, DateTimeKind.Utc),
                    Direction = direction,
                    Amount = r.Amount,
                    Description = r.Description,
                    Currency = r.Currency,
                    Mcc = (Mcc?)r.Mcc,
                    Kind = kind
                   };
                    
                    tx.AssignCategory(r.CatCode);
                    
             return tx;
        }).ToList();


        await _repo.AddRangeAsync(entities);
        await _repo.SaveChangesAsync();

        return entities.Count;
    }
}