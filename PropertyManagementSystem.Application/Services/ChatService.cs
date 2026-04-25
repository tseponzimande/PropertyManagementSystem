namespace PropertyManagementSystem.Application.Services
{
    public class ChatService(
        IMessageService messageService,
        IUserService userService,
        ILogger<ChatService> logger) : IChatService
    {
        private readonly IMessageService _messageService = messageService;
        private readonly IUserService _userService = userService;
        private readonly ILogger<ChatService> _logger = logger;

        public async Task<ConversationDto> GetConversationAsync(Guid userId, Guid otherUserId)
        {
            if (userId == Guid.Empty || otherUserId == Guid.Empty)
                throw new ArgumentException("Invalid user IDs");

            var sent = await _messageService.GetMessagesBySenderAsync(userId);
            var received = await _messageService.GetMessagesByReceiverAsync(userId);

            var messages = sent.Where(m => m.ReceiverId == otherUserId)
                .Concat(received.Where(m => m.SenderId == otherUserId))
                .OrderBy(m => m.CreatedAt)
                .ToList();

            return new ConversationDto
            {
                UserId = userId,
                OtherUserId = otherUserId,
                MessageCount = messages.Count,
                Messages = messages
            };
        }

        public async Task<ConversationsListDto> GetUserConversationsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            var sent = await _messageService.GetMessagesBySenderAsync(userId);
            var received = await _messageService.GetMessagesByReceiverAsync(userId);

            var userIds = sent.Select(m => m.ReceiverId)
                .Concat(received.Select(m => m.SenderId))
                .Distinct();

            var conversations = new List<ConversationPreviewDto>();

            foreach (var otherUserId in userIds)
            {
                var messages = sent.Where(m => m.ReceiverId == otherUserId)
                    .Concat(received.Where(m => m.SenderId == otherUserId))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();

                if (!messages.Any()) continue;

                var last = messages.First();
                var otherUser = await _userService.GetUserByIdAsync(otherUserId);

                conversations.Add(new ConversationPreviewDto
                {
                    UserId = otherUserId,
                    UserName = otherUser?.Name ?? "Unknown",
                    UserEmail = otherUser?.Email ?? "",
                    UserRole = otherUser?.Role ?? "",
                    LastMessage = last.Content,
                    LastMessageTime = last.CreatedAt,
                    MessageCount = messages.Count,
                    UnreadCount = messages.Count(m => m.ReceiverId == userId && !m.IsRead),
                    HasAttachment = !string.IsNullOrEmpty(last.AttachmentUrl),
                    IsLastMessageFromMe = last.SenderId == userId
                });
            }

            return new ConversationsListDto
            {
                UserId = userId,
                ConversationCount = conversations.Count,
                Conversations = conversations.OrderByDescending(c => c.LastMessageTime).ToList()
            };
        }

        public async Task<MessageDto?> SendMessageAsync(MessageDto dto)
        {
            if (dto.SenderId == Guid.Empty || dto.ReceiverId == Guid.Empty)
                throw new ArgumentException("Invalid sender or receiver ID");

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Message content cannot be empty");

            return await _messageService.SendMessageAsync(dto);
        }

        public async Task<MessageDto?> SendMessageWithAttachmentAsync(MessageDto dto, IFormFile attachment)
        {
            if (dto.SenderId == Guid.Empty || dto.ReceiverId == Guid.Empty)
                throw new ArgumentException("Invalid sender or receiver ID");

            if (attachment == null || attachment.Length == 0)
                throw new ArgumentException("Attachment is required");

            return await _messageService.SendMessageWithAttachmentAsync(dto, attachment);
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("Invalid message ID");

            return await _messageService.MarkMessageAsReadAsync(messageId);
        }

        public async Task<bool> MarkConversationAsReadAsync(Guid userId, Guid otherUserId)
        {
            if (userId == Guid.Empty || otherUserId == Guid.Empty)
                throw new ArgumentException("Invalid user IDs");

            return await _messageService.MarkConversationAsReadAsync(userId, otherUserId);
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            return await _messageService.GetUnreadMessageCountAsync(userId);
        }

        public async Task<SearchMessagesResultDto> SearchMessagesAsync(Guid userId, string searchTerm)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term is required");

            var sent = await _messageService.GetMessagesBySenderAsync(userId);
            var received = await _messageService.GetMessagesByReceiverAsync(userId);

            var matches = sent.Concat(received)
                .Where(m => m.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return new SearchMessagesResultDto
            {
                UserId = userId,
                SearchTerm = searchTerm,
                ResultCount = matches.Count,
                Messages = matches
            };
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("Invalid message ID");

            return await _messageService.DeleteMessageAsync(messageId);
        }

        public async Task<bool> DeleteConversationAsync(Guid userId, Guid otherUserId)
        {
            if (userId == Guid.Empty || otherUserId == Guid.Empty)
                throw new ArgumentException("Invalid user IDs");

            var sent = await _messageService.GetMessagesBySenderAsync(userId);
            var received = await _messageService.GetMessagesByReceiverAsync(userId);

            var messages = sent.Where(m => m.ReceiverId == otherUserId)
                .Concat(received.Where(m => m.SenderId == otherUserId))
                .ToList();

            foreach (var msg in messages)
            {
                await _messageService.DeleteMessageAsync(msg.Id);
            }

            return true;
        }
    }
}