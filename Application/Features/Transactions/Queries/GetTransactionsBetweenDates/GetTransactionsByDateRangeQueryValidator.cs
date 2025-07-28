using FluentValidation;

public class GetTransactionsByDateRangeQueryValidator
    : AbstractValidator<GetTransactionsByDateRangeQuery>
{
    public GetTransactionsByDateRangeQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be on or after StartDate");
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);


        When(x => x.Kinds != null && x.Kinds.Any(), () =>
        {
            RuleForEach(x => x.Kinds)
                .IsInEnum()
                .WithMessage("'{PropertyValue}' is not a valid transaction kind.");
        });
    }
}
