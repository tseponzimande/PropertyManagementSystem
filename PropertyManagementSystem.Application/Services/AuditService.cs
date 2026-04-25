using PropertyManagementSystem.Application.DTOs.AuditLog;

namespace PropertyManagementSystem.Application.Services
{
    public class AuditService(IUnitOfWork unitOfWork,IMapper mapper,ILogger<AuditService> logger) : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuditService> _logger = logger;

        public async Task LogAsync(string action,string entityName,Guid? entityId,Guid userId,string? oldValue = null,string? newValue = null,string? ipAddress = null,string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    OldValue = oldValue,
                    NewValue = newValue,
                    UserId = userId,
                    IpAddress = ipAddress ?? "Unknown",
                    UserAgent = userAgent,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.AuditLog.CreateAsync(auditLog);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation(
                    "Audit log created: {Action} on {EntityName} by user {UserId}",
                    action, entityName, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log");
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByUserAsync(Guid userId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var auditLogs = await _unitOfWork.AuditLog.GetByUserAsync(userId);
                var paged = auditLogs
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return MapAuditLogs(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for user {UserId}", userId);
                return Enumerable.Empty<AuditLogDto>();
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName, Guid entityId)
        {
            try
            {
                var auditLogs = await _unitOfWork.AuditLog.GetByEntityAsync(entityName, entityId);
                return MapAuditLogs(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving audit logs for entity {EntityName} with ID {EntityId}",
                    entityName, entityId);
                return Enumerable.Empty<AuditLogDto>();
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByActionAsync(string action)
        {
            try
            {
                var auditLogs = await _unitOfWork.AuditLog.GetByActionAsync(action);
                return MapAuditLogs(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for action {Action}", action);
                return Enumerable.Empty<AuditLogDto>();
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByDateRangeAsync(
            DateTime startDate, DateTime endDate)
        {
            try
            {
                var auditLogs = await _unitOfWork.AuditLog.GetByDateRangeAsync(startDate, endDate);
                return MapAuditLogs(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving audit logs for date range {StartDate} to {EndDate}",
                    startDate, endDate);
                return Enumerable.Empty<AuditLogDto>();
            }
        }

        private IEnumerable<AuditLogDto> MapAuditLogs(IEnumerable<AuditLog> auditLogs)
        {
            return auditLogs.Select(a => new AuditLogDto
            {
                Id = a.Id,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                UserId = a.UserId,
                UserName = a.User?.Name ?? "Unknown",
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var auditLogs = await _unitOfWork.AuditLog.GetAllAsync();

                var paged = auditLogs
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return MapAuditLogs(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving audit logs (Page {PageNumber}, PageSize {PageSize})",
                    pageNumber, pageSize);

                return Enumerable.Empty<AuditLogDto>();
            }
        }

    }
}