namespace PropertyManagementSystem.Application.DTOs
{
    public class ConversationPreviewDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string UserRole { get; set; } = null!;
        public string LastMessage { get; set; } = null!;
        public DateTime LastMessageTime { get; set; }
        public int MessageCount { get; set; }
        public int UnreadCount { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsLastMessageFromMe { get; set; }
    }
}
