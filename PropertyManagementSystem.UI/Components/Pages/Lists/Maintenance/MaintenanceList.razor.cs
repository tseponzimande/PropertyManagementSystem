namespace PropertyManagementSystem.UI.Components.Pages.Lists.Maintenance
{
    public partial class MaintenanceList
    {
        #region Dependencies

        [Inject]
        private IMaintenanceService MaintenanceService { get; set; } = null!;

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private ILeaseService LeaseService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        #endregion

        #region Fields

        private List<MaintenanceRequestDto> all = new();

        private List<MaintenanceRequestDto> rows = new();

        private List<UnitDto> _units = new();

        private bool loading = true;

        private string? error = null;

        private string statusFilter = string.Empty;

        private string subtitle = "All maintenance requests";

        private bool isTenant;

        private Guid currentUserId;

        private List<Guid> tenantUnitIds = new();

        private static Func<string, List<string>> nextStatuses = current => current switch
        {
            "Open" => new() { "Open", "InProgress" },
            "InProgress" => new() { "Open", "InProgress", "Resolved" },
            _ => new() { current }
        };

        #endregion

        #region LifyCycle Method

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();

            currentUserId = state.User.GetUserId();

            isTenant = state.User.IsTenant();

            if (isTenant) 
                subtitle = "Your submitted maintenance requests";

            await Load();
        }

        #endregion


        #region Method

        private async Task Load()
        {
            loading = true;

            error = null;
            try
            {
                _units = (await UnitService.GetAllUnitsAsync()).ToList();

                if (isTenant)
                {
                    var leases = await LeaseService.GetLeasesByTenantAsync(currentUserId);
                    tenantUnitIds = leases.Select(l => l.UnitId).ToList();

                    all = new();
                    foreach (var unitId in tenantUnitIds)
                    {
                        var reqs = await MaintenanceService.GetRequestsByUnitAsync(unitId);
                        all.AddRange(reqs);
                    }
                }
                else
                {
                    all = new();
                    foreach (var unit in _units)
                    {
                        var reqs = await MaintenanceService.GetRequestsByUnitAsync(unit.Id);
                        all.AddRange(reqs);
                    }
                }

                ApplyFilter(null);
            }
            catch (Exception ex) 
            { 
                error = "Failed to load maintenance requests."; Console.Error.WriteLine(ex);
            }

            finally 
            { 
                loading = false; 
            }
        }

        private void ApplyFilter(object? _)
        {
            rows = string.IsNullOrEmpty(statusFilter) ? all.ToList() : all.Where(m => m.Status == statusFilter).ToList();
        }

        private async Task UpdateStatus(MaintenanceRequestDto dto, string newStatus)
        {
            try
            {
                var ok = await MaintenanceService.UpdateRequestStatusAsync(dto.Id, newStatus);
                if (ok)
                {
                    dto.Status = newStatus;

                    NotificationService.Notify(NotificationSeverity.Success, "Updated", $"'{dto.Title}' → {newStatus}");

                    StateHasChanged();
                }
                else NotificationService.Notify(NotificationSeverity.Error, "Error", "Status update failed.");
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Status update failed.");
            }
        }

        private async Task OpenCreate()
        {
            var defaultUnitId = isTenant && tenantUnitIds.Any() ? tenantUnitIds.First() : (_units.FirstOrDefault()?.Id ?? Guid.Empty);

            var dto = new MaintenanceRequestDto
            {
                UnitId = defaultUnitId,
                TenantId = currentUserId,
                Status = "Open"
            };

            var ok = await DialogService.OpenAsync<MaintenanceFormDialog>(
            "New Maintenance Request",
                new() { ["Model"] = dto, ["Units"] = isTenant ? _units.Where(u => tenantUnitIds.Contains(u.Id)).ToList() : _units },
                new DialogOptions { Width = "480px", Draggable = true });

            if (ok is true)
                await Load();
        }

        #endregion
    }
}
