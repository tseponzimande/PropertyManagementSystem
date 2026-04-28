namespace PropertyManagementSystem.UI.Components.Pages.Maintenance
{
    public partial class MaintenanceDetail
    {
        #region Dependencies

        [Inject]
        private IMaintenanceService MaintenanceService { get; set; } = null!;

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Parameter

        [Parameter]
        public Guid Id { get; set; }

        private MaintenanceRequestDto? request = null;
        private string unitNumber = "—";
        private string newStatus = string.Empty;
        private bool loading = true;
        private bool saving = false;

        private readonly List<string> statusOptions = new() { "Open", "InProgress", "Resolved" };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var units = (await UnitService.GetAllUnitsAsync()).ToList();

                foreach (var unit in units)
                {
                    var reqs = await MaintenanceService.GetRequestsByUnitAsync(unit.Id);
                    request = reqs.FirstOrDefault(r => r.Id == Id);
                    if (request is not null)
                    {
                        unitNumber = unit.UnitNumber;
                        newStatus = request.Status;
                        break;
                    }
                }
            }
            finally
            {
                loading = false;
            }
        }

        private async Task SaveStatus()
        {
            if (request is null || newStatus == request.Status)
                return;

            saving = true;

            try
            {
                var ok = await MaintenanceService.UpdateRequestStatusAsync(Id, newStatus);
                if (ok)
                {
                    request.Status = newStatus;
                    NotificationService.Notify(NotificationSeverity.Success, "Updated", $"Status → {newStatus}");
                }
                else NotificationService.Notify(NotificationSeverity.Error, "Error", "Update failed.");
            }
            finally
            {
                saving = false;
            }
        }

        #endregion

    }
}
