namespace Application.DTOs;

public record SpendingAnalyticsDto( string CategoryCode, string CategoryName, string SubcategoryCode, string SubcategoryName, decimal TotalAmount);
