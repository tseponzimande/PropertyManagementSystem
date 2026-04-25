namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetMessagesBySenderAsync(Guid senderId);
        Task<IEnumerable<MessageDto>> GetMessagesByReceiverAsync(Guid receiverId);
        Task<MessageDto?> SendMessageAsync(MessageDto dto);
        Task<MessageDto?> CreateMessageAsync(MessageDto messageDto);
        Task<MessageDto?> SendMessageWithAttachmentAsync(MessageDto dto, IFormFile attachment);
        Task<bool> MarkMessageAsReadAsync(Guid messageId);
        Task<bool> MarkConversationAsReadAsync(Guid userId, Guid otherUserId);
        Task<int> GetUnreadMessageCountAsync(Guid userId);
        Task<bool> DeleteMessageAsync(Guid messageId);
    }
}
