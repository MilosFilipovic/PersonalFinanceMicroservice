using Domain.Interfaces;
using MediatR;

namespace Application.Features.Analytics.Queries.GetSpendingAnalytics
{
    public class GetSpendingAnalyticsQueryHandler : IRequestHandler<GetSpendingAnalyticsQuery, SpendingAnalyticsResponse>
    {
        private readonly ITransactionRepository _txRepo;
        private readonly ICategoryRepository _catRepo;
        

        public GetSpendingAnalyticsQueryHandler(
            ITransactionRepository txRepo,
            ICategoryRepository catRepo)
        {
            _txRepo = txRepo;
            _catRepo = catRepo;
        }

        public async Task<SpendingAnalyticsResponse> Handle(GetSpendingAnalyticsQuery request, CancellationToken ct)
        {

            DateTime? startUtc = request.StartDate.HasValue
                        ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc)
                        : (DateTime?)null;

            DateTime? endUtc = request.EndDate.HasValue
                        ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                        : (DateTime?)null;

            
            var txs = await _txRepo.GetForAnalyticsAsync(
                categoryCode: request.CategoryCode,
                startDate: startUtc,
                endDate: endUtc,
                direction: request.Direction,
                ct: ct);


            var result = txs
                    .GroupBy(t =>
                    {
                        var code = t.CatCode ?? t.Category?.Code ?? "UNKNOWN";
                        return code;
                    })
                    .Select(codeGroup =>
                        new SpendingGroup(
                            CatCode: codeGroup.Key,
                            Amount: codeGroup.Sum(t => t.Amount),
                            Count: codeGroup.Count()
                        )
                    )
                    .OrderByDescending(g => g.Amount)
                    .ToList();

            return new SpendingAnalyticsResponse(result);            // wrap
        }

        
    }
}
