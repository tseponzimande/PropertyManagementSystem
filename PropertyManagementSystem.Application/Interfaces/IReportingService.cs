namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IReportingService
    {
        Task<FinancialReportDto> GenerateFinancialReportAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null);
        Task<OccupancyReportDto> GenerateOccupancyReportAsync(Guid? propertyId = null);
        Task<MaintenanceReportDto> GenerateMaintenanceReportAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null);
        Task<TenantReportDto> GenerateTenantReportAsync(Guid? ownerId = null);
        Task<LeaseExpiryReportDto> GenerateLeaseExpiryReportAsync(int daysAhead = 90);
        Task<byte[]> ExportFinancialReportToCsvAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null);
    }
}
