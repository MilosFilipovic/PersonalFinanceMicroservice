using Application.Features.Transactions.Commands.SplitTransaction;
using Domain.Interfaces;
using FluentValidation;

public class SplitTransactionCommandValidator
    : AbstractValidator<SplitTransactionCommand>
{
    public SplitTransactionCommandValidator(ITransactionRepository repo)

    {
        // Postoji li transakcija
        RuleFor(cmd => cmd.TransactionId)
            .NotEmpty()
            .WithMessage("Id transakcije je obvezan.")
            .MustAsync(async (id, ct) => await repo.ExistsAsync(id))
            .WithMessage(id => $"Transakcija s Id = '{id}' ne postoji.");

        // Splits lista postoji i ima barem jedan element
        RuleFor(cmd => cmd.Splits)
            .NotNull()
            .WithMessage("Morate poslati splits listu.")
            .NotEmpty()
            .WithMessage("Splits lista ne može biti prazna.");

        // validacija svakog itema
        RuleForEach(cmd => cmd.Splits)
            .SetValidator(new SplitItemDtoValidator());

        // zbir tacno odgovara iznosu transakcije
        RuleFor(cmd => cmd)
            .MustAsync(async (cmd, ct) =>
            {
                var tx = await repo.GetByIdAsync(cmd.TransactionId);
                var totalSplit = cmd.Splits.Sum(s => s.Amount);
                return tx != null && totalSplit == tx.Amount;
            })
            .WithMessage(cmd =>
            {
                
                var txAmount = repo.GetByIdAsync(cmd.TransactionId).Result.Amount;
                var sum = cmd.Splits.Sum(s => s.Amount);
                return $"Zbir splita ({sum}) ne odgovara iznosu transakcije ({txAmount}).";
            })
            .WhenAsync(async (cmd, ct) => await repo.ExistsAsync(cmd.TransactionId));
    }
}