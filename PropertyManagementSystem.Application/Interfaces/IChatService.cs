namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IChatService
    {
        Task<ConversationDto> GetConversationAsync(Guid userId, Guid otherUserId);
        Task<ConversationsListDto> GetUserConversationsAsync(Guid userId);
        Task<MessageDto?> SendMessageAsync(MessageDto dto);
        Task<MessageDto?> SendMessageWithAttachmentAsync(MessageDto dto, IFormFile attachment);
        Task<bool> MarkMessageAsReadAsync(Guid messageId);
        Task<bool> MarkConversationAsReadAsync(Guid userId, Guid otherUserId);
        Task<int> GetUnreadMessageCountAsync(Guid userId);
        Task<SearchMessagesResultDto> SearchMessagesAsync(Guid userId, string searchTerm);
        Task<bool> DeleteMessageAsync(Guid messageId);
        Task<bool> DeleteConversationAsync(Guid userId, Guid otherUserId);
    }
}
