namespace PropertyManagementSystem.Domain.Entities
{
    public class Property : BaseEntity
    {
        #region Properties
        public string Address { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;

        public ICollection<Unit> Units { get; set; } = new List<Unit>();

        #endregion

        #region Foreign Keys
        public Guid OwnerId { get; set; }
        #endregion

        #region Navigation Properties
        public User Owner { get; set; } = null!;
        #endregion
    }
}
