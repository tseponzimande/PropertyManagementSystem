namespace PropertyManagementSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        #region Properties

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        #endregion

        #region Foreign Keys

        public Guid RoleId { get; set; }

        #endregion

        #region Navigation Properties

        public Role Role { get; set; } = null!;

        #endregion

    }
}
