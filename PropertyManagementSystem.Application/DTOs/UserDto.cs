namespace PropertyManagementSystem.Application.DTOs
{
    #region User 
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
    #endregion
}
