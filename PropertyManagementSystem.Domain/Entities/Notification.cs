namespace PropertyManagementSystem.Domain.Entities
{
    public class Notification: BaseEntity
    {
        #region Scalar Properties
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        #endregion

        #region Foreign Keys
        public Guid UserId { get; set; }
        #endregion

        #region Navigation Properties
        public User User { get; set; } = null!;
        #endregion
    }
}
