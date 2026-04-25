namespace PropertyManagementSystem.Domain.Entities
{
    public class Message : BaseEntity
    {
        #region Properties
        public string Content { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public string? AttachmentUrl { get; set; }

        public string? AttachmentFileName { get; set; }

        public long? AttachmentFileSize { get; set; }

        #endregion

        #region Foreign Keys
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        #endregion

        #region Navigation Properties
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        #endregion
    }
}
