using PropertyManagementSystem.Application.DTOs.AuditLog;

namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityName, Guid? entityId, Guid userId,
            string? oldValue = null, string? newValue = null, string? ipAddress = null, string? userAgent = null);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByUserAsync(Guid userId, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName, Guid entityId);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByActionAsync(string action);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

    }
}