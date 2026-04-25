namespace PropertyManagementSystem.Infrastructure.SignalR
{
    [Authorize]
    public class NotificationHub(INotificationService notificationService, ILogger<NotificationHub> logger) : Hub
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<NotificationHub> _logger = logger;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User {UserId} connected to NotificationHub", userId);

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
            {
                var unreadNotifications = await _notificationService.GetUnreadNotificationsByUserAsync(userGuid);
                await Clients.Caller.SendAsync("UnreadCount", unreadNotifications.Count());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(Guid userId, string type, string message)
        {
            try
            {
                var notificationDto = new NotificationDto
                {
                    UserId = userId,
                    Type = type,
                    Message = message,
                    IsRead = false
                };

                var savedNotification = await _notificationService.CreateNotificationAsync(notificationDto);

                if (savedNotification != null)
                {
                    await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", savedNotification);

                    var unreadNotifications = await _notificationService.GetUnreadNotificationsByUserAsync(userId);
                    await Clients.User(userId.ToString()).SendAsync("UnreadCount", unreadNotifications.Count());

                    _logger.LogInformation("Notification sent to user {UserId}: {Type}", userId, type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            }
        }

        public async Task BroadcastNotification(string type, string message)
        {
            try
            {
                var userRole = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    _logger.LogWarning("Unauthorized broadcast attempt by non-admin user");
                    await Clients.Caller.SendAsync("Error", "Unauthorized");
                    return;
                }

                await Clients.All.SendAsync("ReceiveBroadcast", new { Type = type, Message = message });
                _logger.LogInformation("Broadcast notification sent: {Type}", type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting notification");
            }
        }

        public async Task MarkNotificationAsRead(Guid notificationId)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(notificationId);

                if (result)
                {
                    var userIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
                    {
                        var unreadNotifications = await _notificationService.GetUnreadNotificationsByUserAsync(userId);
                        await Clients.Caller.SendAsync("UnreadCount", unreadNotifications.Count());
                    }

                    await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
            }
        }

        public async Task GetUnreadCount()
        {
            try
            {
                var userIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr))
                    return;

                var userId = Guid.Parse(userIdStr);
                var unreadNotifications = await _notificationService.GetUnreadNotificationsByUserAsync(userId);

                await Clients.Caller.SendAsync("UnreadCount", unreadNotifications.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
            }
        }
    }
}