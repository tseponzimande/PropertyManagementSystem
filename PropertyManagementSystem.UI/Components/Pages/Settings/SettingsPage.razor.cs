namespace PropertyManagementSystem.UI.Components.Pages.Settings
{
    public partial class SettingsPage
    {
        #region Dependencies

        [Inject]
        private IAnalyticsService AnalyticsService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields

        private SystemAnalyticsDto? analytics = null;
        private GrowthMetricsDto? growth = null;
        private MaintenanceTrendsDto? trends = null;

        private bool loadingAnalytics = true;
        private bool loadingGrowth = false;
        private bool loadingTrends = false;
        private string? analyticsError = null;
        private string? growthError = null;
        private string? trendsError = null;

        private readonly List<AdminLink> adminLinks = new()
        {
            new("Users", "/users", "manage_accounts", "Manage user accounts and roles"),
            new("Properties", "/properties", "home_work", "Review and approve properties"),
            new("Audit Logs",  "/audit", "manage_search",  "Full system activity trail"),
            new("Reports",  "/reports", "bar_chart", "Financial and operational reports"),
            new("Maintenance", "/maintenance", "build", "Track all maintenance requests"),
            new("Leases", "/leases", "description", "Manage all active leases"),
            new("Notifications", "/notifications", "notifications",   "Your alerts and messages"),
            new("Chat", "/chat", "chat", "Messaging between users"),
        };
        private record AdminLink(string Title, string Url, string Icon, string Description);

        #endregion

        #region lifeCcyle Methods

        protected override async Task OnInitializedAsync()
            => await LoadAnalytics();

        #endregion

        #region Methods

        private async Task OnTabChanged(int tab)
        {
            if (tab == 1 && growth is null && !loadingGrowth)
                await LoadGrowth();
            else if (tab == 2 && trends is null && !loadingTrends)
                await LoadTrends();
        }

        private async Task LoadAnalytics()
        {
            loadingAnalytics = true;

            analyticsError = null;

            try
            {
                analytics = await AnalyticsService.GetSystemAnalyticsAsync();
            }
            catch
            {
                analyticsError = "Could not load analytics.";
            }
            finally
            {
                loadingAnalytics = false;
            }
        }

        private async Task LoadGrowth()
        {
            loadingGrowth = true;

            growthError = null;

            try
            {
                growth = await AnalyticsService.GetGrowthMetricsAsync();
            }
            catch
            {
                growthError = "Could not load growth metrics.";
            }
            finally
            {
                loadingGrowth = false;
            }
        }

        private async Task LoadTrends()
        {
            loadingTrends = true;

            trendsError = null;

            try
            {
                trends = await AnalyticsService.GetMaintenanceTrendsAsync(6);
            }
            catch
            {
                trendsError = "Could not load maintenance trends.";
            }
            finally
            {
                loadingTrends = false;
            }
        }

        private static string GC(double r) => r >= 5 ? "Success" : r >= 0 ? "Info" : "Danger";

        #endregion
    }
}
