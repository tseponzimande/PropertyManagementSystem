namespace PropertyManagementSystem.UI.Components.Shared;

public partial class NotificationBell
{
    [Inject]
    private INotificationService _NotificationService { get; set; } = default!;

    [Inject]
    private AuthStateService _AuthStateService { get; set; } = default!;

    [Inject]
    private NavigationManager _NavigationManager { get; set; } = default!;

    [Inject]
    private AuthStateService _AuthState { get; set; } = default!;

    private bool panelOpen;

    private bool isLoading;

    private int unreadCount;

    private List<NotificationDto> notifications = new();

    private Guid userId;

    private bool _loaded;


    private async Task LoadInitialData()
    {
        try
        {
            var all = await _NotificationService.GetNotificationsByUserAsync(userId);
            notifications = all.OrderByDescending(n => n.CreatedAt).Take(10).ToList();
            unreadCount = notifications.Count(n => !n.IsRead);
        }
        catch 
        { }
    }

    private void TogglePanel() => panelOpen = !panelOpen;

    private async Task LoadNotificationsAsync()
    {
        isLoading = true;

        try
        {
            var all = await _NotificationService.GetNotificationsByUserAsync(userId);
            notifications = all.OrderByDescending(n => n.CreatedAt).Take(10).ToList();
            unreadCount = notifications.Count(n => !n.IsRead);
        }
        catch
        {
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task MarkAsRead(NotificationDto n)
    {
        if (n.IsRead)
            return;

        await _NotificationService.MarkAsReadAsync(n.Id);
        n.IsRead = true;

        unreadCount = Math.Max(0, unreadCount - 1);
    }

    private async Task MarkAllRead()
    {
        foreach (var n in notifications.Where(x => !x.IsRead))
        {
            await _NotificationService.MarkAsReadAsync(n.Id);
            n.IsRead = true;
        }

        unreadCount = 0;
    }

    private void GoToNotifications()
    {
        panelOpen = false;
        _NavigationManager.NavigateTo("/notifications");
    }

    private static string GetIcon(string type) => type?.ToLower() switch
    {
        "payment" or "payment_due" => "payments",
        "lease" or "lease_expiry" => "description",
        "maintenance" or "maintenance_update" => "build",
        "message" => "chat",
        _ => "info"
    };

    private async Task OpenNotifications()
    {
        await LoadNotificationsAsync();
        panelOpen = true;
    }
}

