namespace PropertyManagementSystem.Application.Services
{
    public class MessageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MessageService> logger,
        IFileUploadService fileUploadService) : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MessageService> _logger = logger;
        private readonly IFileUploadService _fileUploadService = fileUploadService;

        public async Task<IEnumerable<MessageDto>> GetMessagesBySenderAsync(Guid senderId)
        {
            try
            {
                var messages = await _unitOfWork.Messages.GetMessagesBySenderAsync(senderId);
                return _mapper.Map<IEnumerable<MessageDto>>(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages for sender {SenderId}", senderId);
                return Enumerable.Empty<MessageDto>();
            }
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesByReceiverAsync(Guid receiverId)
        {
            try
            {
                var messages = await _unitOfWork.Messages.GetMessagesByReceiverAsync(receiverId);
                return _mapper.Map<IEnumerable<MessageDto>>(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages for receiver {ReceiverId}", receiverId);
                return Enumerable.Empty<MessageDto>();
            }
        }

        public async Task<MessageDto?> SendMessageAsync(MessageDto dto)
        {
            try
            {
                var message = _mapper.Map<Message>(dto);
                message.Id = Guid.NewGuid();
                message.IsRead = false;
                message.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Messages.CreateAsync(message);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<MessageDto>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message");
                return null;
            }
        }

        public async Task<MessageDto?> CreateMessageAsync(MessageDto messageDto)
        {
            try
            {
                var message = _mapper.Map<Message>(messageDto);
                message.Id = Guid.NewGuid();
                message.IsRead = false;
                message.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Messages.CreateAsync(message);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<MessageDto>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create message");
                return null;
            }
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId)
        {
            try
            {
                var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
                if (message == null)
                {
                    _logger.LogWarning("Message {MessageId} not found", messageId);
                    return false;
                }

                if (!message.IsRead)
                {
                    message.IsRead = true;
                    await _unitOfWork.Messages.UpdateAsync(message);
                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation("Message {MessageId} marked as read", messageId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark message {MessageId} as read", messageId);
                return false;
            }
        }

        public async Task<bool> MarkConversationAsReadAsync(Guid userId, Guid otherUserId)
        {
            try
            {
                var messages = await _unitOfWork.Messages.GetMessagesByReceiverAsync(userId);
                var unreadMessages = messages
                    .Where(m => m.SenderId == otherUserId && !m.IsRead)
                    .ToList();

                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                    await _unitOfWork.Messages.UpdateAsync(message);
                }

                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Marked {Count} messages as read in conversation between {UserId} and {OtherUserId}",
                    unreadMessages.Count, userId, otherUserId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark conversation as read");
                return false;
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId)
        {
            try
            {
                var messages = await _unitOfWork.Messages.GetMessagesByReceiverAsync(userId);
                var unreadCount = messages.Count(m => !m.IsRead);
                return unreadCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread message count for user {UserId}", userId);
                return 0;
            }
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            try
            {
                var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
                if (message == null)
                {
                    _logger.LogWarning("Message {MessageId} not found", messageId);
                    return false;
                }

                if (!string.IsNullOrEmpty(message.AttachmentUrl))
                {
                    await _fileUploadService.DeleteFileAsync(message.AttachmentUrl);
                }

                await _unitOfWork.Messages.DeleteAsync(messageId);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Message {MessageId} deleted successfully", messageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete message {MessageId}", messageId);
                return false;
            }
        }

        public async Task<MessageDto?> SendMessageWithAttachmentAsync(MessageDto dto, IFormFile attachment)
        {
            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".txt", ".zip" };
                var maxFileSize = 10 * 1024 * 1024;

                if (!_fileUploadService.IsValidFileType(attachment, allowedExtensions))
                {
                    _logger.LogWarning("Invalid file type: {FileName}", attachment.FileName);
                    return null;
                }

                if (!_fileUploadService.IsValidFileSize(attachment, maxFileSize))
                {
                    _logger.LogWarning("File too large: {FileName}", attachment.FileName);
                    return null;
                }

                var fileUrl = await _fileUploadService.UploadFileAsync(attachment, "messages");

                var message = _mapper.Map<Message>(dto);
                message.Id = Guid.NewGuid();
                message.IsRead = false;
                message.CreatedAt = DateTime.UtcNow;
                message.AttachmentUrl = fileUrl;
                message.AttachmentFileName = attachment.FileName;
                message.AttachmentFileSize = attachment.Length;

                await _unitOfWork.Messages.CreateAsync(message);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Message with attachment sent successfully: {MessageId}", message.Id);
                return _mapper.Map<MessageDto>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message with attachment");
                return null;
            }
        }
    }
}