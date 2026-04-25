namespace PropertyManagementSystem.Application.DTOs
{
    public class CreateAuditLogDto
    {
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public Guid? EntityId { get; set; }
        public Guid UserId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
