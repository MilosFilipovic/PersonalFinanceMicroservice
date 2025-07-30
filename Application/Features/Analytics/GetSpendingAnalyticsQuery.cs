using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Analytics;

public record GetSpendingAnalyticsQuery(
    string? CategoryCode,
    DateTime? StartDate,
    DateTime? EndDate,
    Direction? Direction
) : IRequest<SpendingAnalyticsResponse>;
