namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetAdminDashboardStatsAsync();
        Task<DashboardStatsDto> GetOwnerDashboardStatsAsync(Guid ownerId);
        Task<DashboardStatsDto> GetTenantDashboardStatsAsync(Guid tenantId);
        Task<RevenueStatsDto> GetRevenueStatsAsync(Guid? ownerId = null);
        Task<IEnumerable<PropertyStatsDto>> GetPropertyStatsAsync(Guid? ownerId = null);
        Task<IEnumerable<MaintenanceStatsDto>> GetMaintenanceStatsAsync();

    }
}
