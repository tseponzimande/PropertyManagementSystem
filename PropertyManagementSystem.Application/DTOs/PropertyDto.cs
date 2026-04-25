namespace PropertyManagementSystem.Application.DTOs
{
    #region Property

    public class PropertyDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;
        public Guid OwnerId { get; set; }
    }

    #endregion
}
