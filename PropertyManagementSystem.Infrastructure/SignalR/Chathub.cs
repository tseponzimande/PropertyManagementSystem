namespace PropertyManagementSystem.Infrastructure.SignalR
{
    //[Authorize]
    [AllowAnonymous]
    public class ChatHub(IMessageService messageService, ILogger<ChatHub> logger) : Hub
    {
        #region Dependencies

        private readonly IMessageService _messageService = messageService;
        private readonly ILogger<ChatHub> _logger = logger;

        #endregion


        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User {UserId} connected to ChatHub", userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User {UserId} disconnected from ChatHub", userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Guid receiverId, string content)
        {
            try
            {
                var senderIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(senderIdStr))
                {
                    _logger.LogWarning("SendMessage failed: Sender ID not found");
                    return;
                }

                var senderId = Guid.Parse(senderIdStr);

                var messageDto = new MessageDto
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content
                };

                var savedMessage = await _messageService.CreateMessageAsync(messageDto);

                if (savedMessage != null)
                {
                    await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", savedMessage);

                    await Clients.Caller.SendAsync("MessageSent", savedMessage);

                    _logger.LogInformation("Message sent from {SenderId} to {ReceiverId}", senderId, receiverId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                await Clients.Caller.SendAsync("Error", "Failed to send message");
            }
        }

        public async Task JoinConversation(Guid otherUserId)
        {
            try
            {
                var currentUserIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdStr))
                    return;

                var currentUserId = Guid.Parse(currentUserIdStr);

                var roomName = string.Compare(currentUserId.ToString(), otherUserId.ToString(), StringComparison.Ordinal) < 0
                    ? $"{currentUserId}_{otherUserId}"
                    : $"{otherUserId}_{currentUserId}";

                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                _logger.LogInformation("User {UserId} joined conversation room {RoomName}", currentUserId, roomName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining conversation");
            }
        }

        public async Task LeaveConversation(Guid otherUserId)
        {
            try
            {
                var currentUserIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdStr))
                    return;

                var currentUserId = Guid.Parse(currentUserIdStr);

                var roomName = string.Compare(currentUserId.ToString(), otherUserId.ToString(), StringComparison.Ordinal) < 0
                    ? $"{currentUserId}_{otherUserId}"
                    : $"{otherUserId}_{currentUserId}";

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                _logger.LogInformation("User {UserId} left conversation room {RoomName}", currentUserId, roomName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conversation");
            }
        }

        public async Task SendTypingIndicator(Guid receiverId)
        {
            try
            {
                var senderIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(senderIdStr))
                    return;

                var senderId = Guid.Parse(senderIdStr);

                await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending typing indicator");
            }
        }
    }
}