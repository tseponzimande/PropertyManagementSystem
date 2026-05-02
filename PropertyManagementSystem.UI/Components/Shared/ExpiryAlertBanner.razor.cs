namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class ExpiryAlertBanner
    {
        #region Dependencies

        [Inject]
        private IReportingService ReportingService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields

        private int count = 0;

        private bool show = false;

        private string? error = null;

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var state = await AuthProvider.GetAuthenticationStateAsync();

                if (!state.User.IsAdmin() || !state.User.IsOwner())
                    return;

                var report = await ReportingService.GenerateLeaseExpiryReportAsync(30);

                count = report.ExpiringLeases;
                show = count > 0;
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Please try Again Later !!!",
                    Duration = 5000
                });
            }
        }

        #endregion
    }
}
