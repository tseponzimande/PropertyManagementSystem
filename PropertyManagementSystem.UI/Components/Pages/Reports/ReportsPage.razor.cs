namespace PropertyManagementSystem.UI.Components.Pages.Reports
{
    public partial class ReportsPage
    {
        #region Dependencies

        [Inject]
        private IReportingService ReportingService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService Toast { get; set; } = null!;

        [Inject]
        private IJSRuntime JS { get; set; } = null!;

        #endregion

        #region Fields and Properties

        private FinancialReportDto? financial = null;

        private OccupancyReportDto? occupancy = null;

        private MaintenanceReportDto? maintenance = null;

        private TenantReportDto? tenants = null;

        private LeaseExpiryReportDto? expiry = null;

        private DateTime finStart = new(DateTime.Now.Year, 1, 1);

        private DateTime finEnd = DateTime.Now;

        private DateTime mntStart = new(DateTime.Now.Year, 1, 1);

        private DateTime mntEnd = DateTime.Now;

        private int expiryDays = 90;

        private bool loadingFin = false;

        private bool loadingOcc = false;

        private bool loadingMnt = false;

        private bool loadingTen = false;

        private bool loadingExp = false;

        private bool exporting = false;

        private Guid currentUserId;

        private bool isOwner;

        private List<PieSlice> occupancyPie = new();

        private record PieSlice(string Label, decimal Value);

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();
            currentUserId = state.User.GetUserId();
            isOwner = state.User.IsOwner();
        }

        #endregion


        #region Methods

        private async Task LoadFinancial()
        {
            loadingFin = true;

            try
            {
                var ownerId = isOwner ? currentUserId : (Guid?)null;

                financial = await ReportingService.GenerateFinancialReportAsync(finStart, finEnd, ownerId);
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
            finally
            {
                loadingFin = false;
            }
        }

        private async Task ExportFinancial()
        {
            exporting = true;

            try
            {
                var ownerId = isOwner ? currentUserId : (Guid?)null;

                var bytes = await ReportingService.ExportFinancialReportToCsvAsync(finStart, finEnd, ownerId);

                var b64 = Convert.ToBase64String(bytes);

                await JS.InvokeVoidAsync("downloadFile", $"Financial_Report_{finStart:yyyyMMdd}_{finEnd:yyyyMMdd}.csv", "text/csv", b64);
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Export failed", ex.Message);
            }

            finally
            {
                exporting = false;
            }
        }

        private async Task LoadOccupancy()
        {
            loadingOcc = true;

            try
            {
                occupancy = await ReportingService.GenerateOccupancyReportAsync();

                occupancyPie = new()
                {
                    new("Occupied", occupancy.OccupiedUnits), new("Available", occupancy.AvailableUnits)
                };
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
            finally
            {
                loadingOcc = false;
            }
        }

        private async Task LoadMaintenance()
        {
            loadingMnt = true;

            try
            {
                var ownerId = isOwner ? currentUserId : (Guid?)null;

                maintenance = await ReportingService.GenerateMaintenanceReportAsync(mntStart, mntEnd, ownerId);
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
            finally
            {
                loadingMnt = false;
            }
        }

        private async Task LoadTenants()
        {
            loadingTen = true;

            try
            {
                var ownerId = isOwner ? currentUserId : (Guid?)null;

                tenants = await ReportingService.GenerateTenantReportAsync(ownerId);
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
            finally
            {
                loadingTen = false;
            }
        }

        private async Task LoadExpiry()
        {
            loadingExp = true;

            try
            {
                expiry = await ReportingService.GenerateLeaseExpiryReportAsync(expiryDays);
            }
            catch (Exception ex)
            {
                Toast.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
            finally
            {
                loadingExp = false;
            }
        }

        private static string DaysColor(int days) =>
            days <= 14 ? "var(--rz-danger)" : days <= 30 ? "var(--rz-warning)" : "var(--rz-text-color)";
    }

    #endregion
}
