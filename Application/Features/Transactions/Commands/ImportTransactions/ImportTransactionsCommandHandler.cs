using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Interfaces;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Domain.Entities.Exceptions;

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

            //// 1) ovde generišeš listu splitova na osnovu r
            //var mySplits = YourSplitLogic(r);

            //// 2) baci BusinessException ako suma splitova > originalni iznos
            //if (mySplits.Sum(s => s.Amount) > r.Amount)
            //    throw new BusinessException(
            //        code: "split-amount-over-transaction-amount",
            //        message: "Zbir razdvojenih iznosa ne može biti veći od originalnog iznosa transakcije."
            //    );

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
                    
                    // 2) dodaj catcode iz CSV-a
                    tx.AssignCategory(r.CatCode);
                    
             return tx;
        }).ToList();


        await _repo.AddRangeAsync(entities);
        await _repo.SaveChangesAsync();

        return entities.Count;
    }
}