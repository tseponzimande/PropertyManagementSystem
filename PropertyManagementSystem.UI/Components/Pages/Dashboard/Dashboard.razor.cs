namespace PropertyManagementSystem.UI.Components.Pages.Dashboard
{
    public partial class Dashboard
    {
        #region Dependencies

        [Inject]
        private IDashboardService DashboardService { get; set; } = null!;

        [Inject]
        private IAnalyticsService AnalyticsService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private AuthStateService AuthState { get; set; } = default!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        #endregion

        #region Fields 

        private DashboardStatsDto? stats = null;

        private PaymentTrendsDto? paymentTrends = null;

        private IEnumerable<PropertyStatsDto>? propertyStats = null;

        private SystemAnalyticsDto? systemAnalytics = null;

        private LoginResponseDto? currentUser = null;

        private bool isLoading = true;
        private bool isLoadingCharts = true;
        private string? errorMessage = null;

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            currentUser = await AuthState.GetCurrentUserAsync();
            await LoadDashboard();
        }

        #endregion

        #region 
        private async Task LoadDashboard()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.IsAdmin())
                    stats = await DashboardService.GetAdminDashboardStatsAsync();

                else if (user.IsOwner())
                    stats = await DashboardService.GetOwnerDashboardStatsAsync(user.GetUserId());

                else
                    stats = await DashboardService.GetTenantDashboardStatsAsync(user.GetUserId());
            }
            catch (Exception ex)
            {
                errorMessage = "Could not load dashboard statistics. Please refresh.";
                NotificationService.Notify(NotificationSeverity.Error, $"Error Occured: {ex.Message}", duration: 4000);
            }
            finally
            {
                isLoading = false;
            }

            await LoadChartsAsync();
        }

        private async Task LoadChartsAsync()
        {
            isLoadingCharts = true;
            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.IsAdmin() || user.IsOwner())
                {
                    var ownerId = user.IsOwner() ? user.GetUserId() : (Guid?)null;

                    paymentTrends = await AnalyticsService.GetPaymentTrendsAsync(12);
                    propertyStats = await DashboardService.GetPropertyStatsAsync(ownerId);
                }

                if (user.IsAdmin())
                    systemAnalytics = await AnalyticsService.GetSystemAnalyticsAsync();
            }
            catch(Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error Occured: {ex.Message}", duration: 4000);
            }
            finally
            {
                isLoadingCharts = false;
            }
        }

        private static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
        }

        private static ProgressBarStyle GetOccupancyStyle(decimal rate) =>
            rate >= 80 ? ProgressBarStyle.Success :
            rate >= 50 ? ProgressBarStyle.Warning :
            ProgressBarStyle.Danger;

        #endregion
    }
}
