namespace PropertyManagementSystem.UI.Components.Pages.Leases
{
    public partial class LeaseDetail
    {
        #region Dependencies

        [Inject]
        private ILeaseService LeaseService { get; set; } = null!;

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get;set; } = null!;

        [Inject]
        private IPaymentService PaymentService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public Guid Id { get; set; }

        #endregion

        #region Fields

        private LeaseDto? lease = null;

        private List<PaymentDto> _payments = new();

        private string unitDisplay = "—";

        private string tenantDisplay = "—";

        private bool loading = true;

        private bool loadingPayments = false;


        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            await LoadLeases();
        }

        private async Task LoadLeases()
        {
            try
            {
                lease = await LeaseService.GetLeaseByIdAsync(Id);

                if (lease is not null)
                {
                    var unit = await UnitService.GetUnitByIdAsync(lease.UnitId);

                    var tenant = await UserService.GetUserByIdAsync(lease.TenantId);

                    unitDisplay = unit?.UnitNumber ?? "—";

                    tenantDisplay = tenant?.Name ?? "—";

                    await LoadPayments();
                }
            }
            finally
            {
                loading = false;
            }
        }

        #endregion

        #region Methods

        private async Task LoadPayments()
        {
            loadingPayments = true;

            try
            {
                _payments = (await PaymentService.GetPaymentsByLeaseAsync(Id)).ToList();
            }
            catch(Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error Occured : {ex.Message}", duration: 4000);
            }
            finally
            {
                loadingPayments = false;
            }
        }

        private string DaysRemaining()
        {
            if (lease is null)
                return "—";

            var days = (int)(lease.EndDate - DateTime.UtcNow).TotalDays;

            return lease.Status != "Active" ? "—" : days > 0 ? $"{days} days" : "Expired";
        }

        private async Task OpenRecordPayment()
        {
            var dto = new PaymentDto 
            { 
                LeaseId = Id,
                Amount = lease?.RentAmount ?? 0 
            };

            var ok = await DialogService.OpenAsync<PaymentFormDialog>(
                "Record Payment",
                new() { ["Model"] = dto },
                new DialogOptions 
                {
                    Width = "400px",Draggable=true
                });

            if (ok is true) 
                await LoadPayments();
        }

        #endregion
    }
}
