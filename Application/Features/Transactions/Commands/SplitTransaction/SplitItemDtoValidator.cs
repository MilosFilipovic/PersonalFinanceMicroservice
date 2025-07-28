using Application.DTOs;
using FluentValidation;

public class SplitItemDtoValidator : AbstractValidator<SplitItemDto>
{
    public SplitItemDtoValidator()
    {
        RuleFor(x => x.CatCode)
            .NotEmpty()
            .WithMessage("CategoryCode je obvezan.");

        RuleFor(x => x.Amount)
            .GreaterThan(0m)
            .WithMessage("Iznos splita mora biti veći od 0.");
    }
}