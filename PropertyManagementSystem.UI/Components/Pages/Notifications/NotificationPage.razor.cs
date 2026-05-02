namespace PropertyManagementSystem.UI.Components.Pages.Notifications
{
    public partial class NotificationPage
    {
        #region Dependencies

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private INotificationService NotificationService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService Toast { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields and Propoerties

        private List<NotificationDto> all = new();

        private List<NotificationDto> rows = new();

        private bool loading = true;

        private bool markingAll = false;

        private string? error = null;

        private string filter = "all";
        private int unread => all.Count(n => !n.IsRead);

        private Guid userId;

        #endregion

        #region LifeCycle Methods
        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();
            userId = state.User.GetUserId();
            await Load();
        }


        #endregion

        #region Methods

        private async Task Load()
        {
            loading = true;

            error = null;

            try
            {
                all = (await NotificationService.GetNotificationsByUserAsync(userId)).ToList();

                ApplyFilter(filter);
            }
            catch (Exception ex)
            {
                error = $"Failed to load notifications. : {ex.Message}";

                Toast.Notify(NotificationSeverity.Error, $"Error: {error}", duration: 4000);
            }

            finally
            {
                loading = false;
            }
        }

        private void ApplyFilter(string? v)
        {
            filter = v ?? "all";

            rows = filter == "unread" ? all.Where(n => !n.IsRead).ToList() : all.OrderByDescending(n => n.CreatedAt).ToList();
        }

        private async Task MarkRead(NotificationDto n)
        {
            if (n.IsRead)
                return;

            try
            {
                var ok = await NotificationService.MarkAsReadAsync(n.Id);

                if (ok)
                {
                    n.IsRead = true;
                    ApplyFilter(filter);
                }
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                Toast.Notify(NotificationSeverity.Error, $"Error: {error}", duration: 4000);
            }
        }

        private async Task MarkAllRead()
        {
            markingAll = true;

            try
            {
                foreach (var n in all.Where(n => !n.IsRead).ToList())
                {
                    await NotificationService.MarkAsReadAsync(n.Id);
                }

                all.ForEach(n => n.IsRead = true);

                ApplyFilter(filter);

                Toast.Notify(NotificationSeverity.Success, "Done", "All notifications marked as read.");
            }
            catch
            {
                Toast.Notify(NotificationSeverity.Error, "Error", "Could not mark all as read.");
            }
            finally
            {
                markingAll = false;
            }
        }

        private static string GetIcon(string type) => type?.ToLower() switch
        {
            "payment" => "payments",
            "maintenance" => "build",
            "lease" => "description",
            "message" => "chat",
            "alert" => "warning",
            _ => "notifications"
        };


        #endregion
    }
}
