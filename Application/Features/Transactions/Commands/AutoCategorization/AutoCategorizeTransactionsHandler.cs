using Application.Common.Models;
using Domain.Entities.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

public class AutoCategorizeTransactionsHandler
    : IRequestHandler<AutoCategorizeTransactionsCommand>
{
    private readonly ITransactionRepository _repo;
    private readonly List<CategorizationRule> _rules;

    public AutoCategorizeTransactionsHandler(
        ITransactionRepository repo,
        IOptions<List<CategorizationRule>> options)
    {
        _repo = repo;
        _rules = options.Value;
    }

    public async Task<Unit> Handle(
        AutoCategorizeTransactionsCommand request,
        CancellationToken ct)
    {
        var txs = await _repo.GetUncategorizedAsync();

        foreach (var rule in _rules)
        {
            
            if (rule.Predicate.Contains("beneficiary-name", StringComparison.OrdinalIgnoreCase))
            {
                // izvuci sve pojmove između '%…%'
                var matches = Regex.Matches(rule.Predicate, "%(.+?)%");
                foreach (Match m in matches)
                {
                    var term = m.Groups[1].Value;
                    foreach (var t in txs.Where(t =>
                         t.BeneficiaryName != null &&
                         t.BeneficiaryName
                          .Contains(term, StringComparison.OrdinalIgnoreCase)))
                    {
                        t.CatCode = rule.CatCode.ToString();
                    }
                }
            }
            else if (rule.Predicate.Contains("mcc", StringComparison.OrdinalIgnoreCase))
            {
                // uzmi broj iza '='
                var num = int.Parse(rule.Predicate
                    .Split('=')[1].Trim());
                var targetMcc = (Mcc)num;
                foreach (var t in txs.Where(t => t.Mcc == targetMcc))
                {
                    t.CatCode = rule.CatCode.ToString();
                }
            }
            // možeš dodati još parse-loga za druge tipove pravila...
        }

        await _repo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
