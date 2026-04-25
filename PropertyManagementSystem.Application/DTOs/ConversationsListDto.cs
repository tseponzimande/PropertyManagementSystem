namespace PropertyManagementSystem.Application.DTOs
{
    public class ConversationsListDto
    {
        public Guid UserId { get; set; }
        public int ConversationCount { get; set; }
        public List<ConversationPreviewDto> Conversations { get; set; } = new();
    }
}
