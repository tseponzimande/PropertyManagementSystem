namespace PropertyManagementSystem.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        #region Properties
        public string Action { get; set; } = null!; 
        public string EntityName { get; set; } = null!; 
        public Guid? EntityId { get; set; }
        public string? OldValue { get; set; } 
        public string? NewValue { get; set; } 
        public string IpAddress { get; set; } = null!;
        public string? UserAgent { get; set; }
        #endregion

        #region Foreign Keys
        public Guid UserId { get; set; }
        #endregion

        #region Navigation Properties
        public User User { get; set; } = null!;
        #endregion
    }
}