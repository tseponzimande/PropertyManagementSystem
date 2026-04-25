namespace PropertyManagementSystem.Domain.Entities
{
    public class RefreshToken
    {
        #region Properties
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
        public string? ReplacedByToken { get; set; }
        public string? RevokedReason { get; set; }
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
