namespace PropertyManagementSystem.Application.DTOs
{
    #region Maintenance Request 
    public class MaintenanceRequestDto
    {
        public Guid Id { get; set; }
        public Guid UnitId { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    #endregion
}
