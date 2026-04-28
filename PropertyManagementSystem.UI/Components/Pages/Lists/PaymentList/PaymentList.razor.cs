namespace PropertyManagementSystem.UI.Components.Pages.Lists.PaymentList
{
    public partial class PaymentList
    {
        #region Dependencies

        [Inject]
        private ILeaseService LeaseService { get; set; } = null!;

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private IPaymentService PaymentService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;
        //private NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Fields

        private List<PaymentDto> all = new();

        private List<PaymentDto> rows = new();

        private List<LeaseDto> leases = new();

        private List<UnitDto> units = new();

        private List<MonthlyData> monthlyData = new();

        private bool loading = true;

        private string? error = null;

        private string statusFilter = string.Empty;

        private Guid? leaseFilter;

        private Guid currentUserId;

        private bool isTenant;

        private readonly DateTime monthStart = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        private record MonthlyData(string Month, decimal Amount);

        #endregion

        #region LifeCycle Methods 

        //protected override async Task OnInitializedAsync()
        //{
        //    var state = await AuthProvider.GetAuthenticationStateAsync();

        //    if(state.User.Identity?.IsAuthenticated == true)
        //    {
        //        currentUserId = state.User.GetUserId();

        //        isTenant = state.User.IsTenant();

        //        await LoadAsync();
        //    }
        //    else
        //    {
        //        error = "you must be logged in to view the payment";
        //        loading = false;
        //    }
        //}

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();

            currentUserId = state.User.GetUserId();

            isTenant = state.User.IsTenant();

            await LoadAsync();
        }

        #endregion

        #region Methods

        private async Task LoadAsync()
        {
            loading = true;

            error = null;

            try
            {
                leases = (await LeaseService.GetAllLeasesAsync()).ToList();
                units = (await UnitService.GetAllUnitsAsync()).ToList();

                if (isTenant)
                    leases = leases.Where(l => l.TenantId == currentUserId).ToList();

                all = new();
                foreach (var lease in leases)
                {
                    var payments = await PaymentService.GetPaymentsByLeaseAsync(lease.Id);
                    all.AddRange(payments);
                }

                monthlyData = all
                    .Where(p => p.Status == PaymentEnum.Completed.ToString() && p.PaymentDate >= DateTime.Now.AddMonths(-6))
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new MonthlyData(new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yy"),g.Sum(p => p.Amount)))
                    .ToList();

                ApplyFilter(null);
            }
            catch (Exception ex)
            {
                error = $"Failed to load payments : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Could Not load Any Payment Sorry !!!",
                    Duration = 5000
                });
            }
            finally
            {
                loading = false;
            }
        }

        private void ApplyFilter(object? _)
        {
            rows = all
                .Where(p => leaseFilter is null || p.LeaseId == leaseFilter)
                .Where(p => string.IsNullOrEmpty(statusFilter) || p.Status == statusFilter)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        private async Task OpenRecord()
        {
            try
            {
                var dto = new PaymentDto();

                if (leaseFilter.HasValue)
                    dto.LeaseId = leaseFilter.Value;

                var ok = await DialogService.OpenAsync<PaymentFormDialog>(
                "Record Payment",
                    new() { ["Model"] = dto },
                    new DialogOptions { Width = "400px" });

                if (ok is true)
                    await LoadAsync();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Could Not Open Dialog Sorry !!!",
                    Duration = 5000
                });
            }
        }

        #endregion
    }
}
