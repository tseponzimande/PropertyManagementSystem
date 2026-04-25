
namespace PropertyManagementSystem.Application.DTOs
{
    public class SendMessageWithAttachmentDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public IFormFile Attachment { get; set; } = default!;
    }
}
