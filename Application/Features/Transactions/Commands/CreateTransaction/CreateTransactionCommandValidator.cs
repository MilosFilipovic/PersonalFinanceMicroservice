using FluentValidation;
using Application.DTOs;

namespace Application.Features.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommandValidator : AbstractValidator<TransactionDto>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date je obavezan.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Date ne može biti u budućnosti.");

            RuleFor(x => x.Direction)
                .IsInEnum().WithMessage("Direction je obavezan i mora biti validna vrijednost.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount mora biti veći od 0.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency je obavezan.")
                .Length(3).WithMessage("Currency mora biti ISO 4217 kod od 3 znaka.");

            RuleFor(x => x.Kind)
                .IsInEnum().WithMessage("Kind je obavezan i mora biti validna vrijednost.");

            // Opcionalni:
            RuleFor(x => x.BeneficiaryName)
                .MaximumLength(200).WithMessage("BeneficiaryName može imati najviše 200 znakova.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description može imati najviše 500 znakova.");
        }
    }
}
