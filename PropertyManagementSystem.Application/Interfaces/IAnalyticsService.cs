namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();
        Task<UserActivityDto> GetUserActivityAsync(Guid userId);
        Task<PaymentTrendsDto> GetPaymentTrendsAsync(int months = 12, Guid? ownerId = null);
        Task<MaintenanceTrendsDto> GetMaintenanceTrendsAsync(int months = 6);
        Task<GrowthMetricsDto> GetGrowthMetricsAsync();
    }
}
