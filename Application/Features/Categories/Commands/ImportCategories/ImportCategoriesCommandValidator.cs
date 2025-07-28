using FluentValidation;

namespace PersonalFinanceApp.Features.Categories.Commands.ImportCategories
{
    public class ImportCategoriesCommandValidator : AbstractValidator<ImportCategoriesCommand>
    {
        public ImportCategoriesCommandValidator()
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
