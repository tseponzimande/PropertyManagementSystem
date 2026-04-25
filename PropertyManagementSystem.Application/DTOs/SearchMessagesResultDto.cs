namespace PropertyManagementSystem.Application.DTOs
{
    public class SearchMessagesResultDto
    {
        public Guid UserId { get; set; }
        public string SearchTerm { get; set; } = null!;
        public int ResultCount { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }
}
