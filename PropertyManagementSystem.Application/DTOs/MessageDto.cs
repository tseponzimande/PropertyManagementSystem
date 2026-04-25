namespace PropertyManagementSystem.Application.DTOs
{
    #region Message
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public string? AttachmentUrl { get; set; }
        public string? AttachmentFileName { get; set; }
        public long? AttachmentFileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    #endregion
}
