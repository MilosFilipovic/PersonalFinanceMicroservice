using FluentValidation;

namespace Application.Features.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsCommandValidator : AbstractValidator<ImportTransactionsCommand>
    {
        public ImportTransactionsCommandValidator()
        {
            RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("Morate da priložite fajl za import");
            
            RuleFor(x => x.FileStream!.Length)
                .GreaterThan(0)
                .WithMessage("Fajl ne može biti prazan");
        }
    }
}
