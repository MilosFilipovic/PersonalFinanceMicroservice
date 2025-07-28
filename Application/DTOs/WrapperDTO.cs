public record SpendingAnalyticsResponse(IEnumerable<SpendingGroup> Groups);
public record SpendingGroup(string CatCode, decimal Amount, int Count);
