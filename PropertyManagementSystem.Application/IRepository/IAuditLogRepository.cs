namespace PropertyManagementSystem.Application.IRepository
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, Guid entityId);
        Task<IEnumerable<AuditLog>> GetByActionAsync(string action);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetPagedAsync(int pageNumber, int pageSize);
    }
}