using Application.DTOs;
using FluentValidation;

namespace Application.Features.Analytics;

public class GetSpendingAnalyticsQueryValidator : AbstractValidator<GetSpendingAnalyticsQuery>
{
    public GetSpendingAnalyticsQueryValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("EndDate must be >= StartDate.");
    }
}
