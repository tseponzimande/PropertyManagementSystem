namespace PropertyManagementSystem.Application.DTOs
{
    public class ConversationDto
    {
        public Guid UserId { get; set; }
        public Guid OtherUserId { get; set; }
        public int MessageCount { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }
}