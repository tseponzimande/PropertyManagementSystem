namespace PropertyManagementSystem.Application.DTOs.AuditLog
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public Guid? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
