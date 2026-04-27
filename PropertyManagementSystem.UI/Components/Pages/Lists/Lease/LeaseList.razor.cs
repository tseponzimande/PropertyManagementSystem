namespace PropertyManagementSystem.UI.Components.Pages.Lists.Lease
{
    public partial class LeaseList
    {
        #region Dependencies

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private ILeaseService LeaseService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

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

        private List<LeaseDto> all = new();

        private List<LeaseDto> rows = new();

        private List<UnitDto> units = new();

        private List<UserDto> users = new();

        private bool loading = true;

        private string? error = null;

        private string statusFilter = string.Empty;

        private string subtitle = "View and manage all lease agreements";

        private bool isTenant;

        private Guid currentUserId;

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();
            currentUserId = state.User.GetUserId();
            isTenant = state.User.IsTenant();

            if (isTenant)
                subtitle = "Your active lease agreements";
            await Load();
        }

        private async Task Load()
        {
            loading = true;

            error = null;
            try
            {
                all = isTenant ? (await LeaseService.GetLeasesByTenantAsync(currentUserId)).ToList() 
                    : (await LeaseService.GetAllLeasesAsync()).ToList();

                units = (await UnitService.GetAllUnitsAsync()).ToList();

                users = (await UserService.GetAllUsersAsync()).ToList();

                ApplyFilter(string.Empty);
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to load leases",
                    Duration = 5000
                });
                
            }
            finally
            {
                loading = false;
            }
        }

        private void ApplyFilter(string? _)
        {
            try
            {
                rows = string.IsNullOrEmpty(statusFilter) ? all.ToList() : all.Where(l => l.Status == statusFilter).ToList();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to Apply filters",
                    Duration = 5000
                });
            }
        }

        private static string ExpiryClass(DateTime endDate)
        {
                var days = (endDate - DateTime.UtcNow).Days;
                return days < 30 ? "expiry-red" : days < 90 ? "expiry-yellow" : "expiry-green";
        }

        private async Task OpenAdd()
        {
            try
            {
                var dto = new LeaseDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddYears(1),
                    Status = LeaseEnum.Active.ToString(),
                };

                var ok = await DialogService.OpenAsync<LeaseFormDialog>(
                "New Lease",
                new()
                {
                    ["Model"] = dto,
                    ["IsEdit"] = false,
                    ["Units"] = units,
                    ["Users"] = users
                },
                    new DialogOptions { Width = "500px", Draggable=true });

                if (ok is true)
                    await Load();
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to Open a dialog",
                    Duration = 5000
                });
            }
        }

        private async Task OpenEdit(LeaseDto l)
        {
            try
            {
                var clone = new LeaseDto
                {
                    Id = l.Id,
                    UnitId = l.UnitId,
                    TenantId = l.TenantId,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    RentAmount = l.RentAmount,
                    Status = l.Status
                };

                var ok = await DialogService.OpenAsync<LeaseFormDialog>(
                    "Edit Lease",
                    new()
                    {
                        ["Model"] = clone,
                        ["IsEdit"] = true,
                        ["Units"] = units,
                        ["Users"] = users
                    },
                    new DialogOptions { Width = "500px", Draggable=true });

                if (ok is true)
                    await Load();
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to Edit",
                    Duration = 5000
                });
            }
        }

        private async Task ConfirmTerminate(LeaseDto l)
        {
            try
            {
                var ok = await DialogService.OpenAsync<ConfirmDialog>(
                    "Terminate Lease",
                    new()
                    {
                        ["Message"] = "Terminate this lease early?",
                        ["SubMessage"] = "Status will change to Terminated. This cannot be undone.",
                        ["ConfirmText"] = "Terminate",
                        ["ConfirmIcon"] = "cancel",
                        ["ConfirmStyle"] = ButtonStyle.Warning,
                        ["Icon"] = "warning_amber",
                        ["IconColor"] = "warning"
                    },
                    new DialogOptions { Width = "420px" });

                if (ok is not true)
                    return;

                var done = await LeaseService.TerminateLeaseAsync(l.Id);

                NotificationService.Notify(done ? NotificationSeverity.Warning : NotificationSeverity.Error, done ? "Terminated" : "Error", done ? "Lease terminated." : "Termination failed.");

                if (done)
                    await Load();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to Confirm Terminate",
                    Duration = 5000
                });
            }
        }

        private async Task ConfirmDelete(LeaseDto l)
        {
            try
            {
                var ok = await DialogService.OpenAsync<ConfirmDialog>(
                    "Delete Lease",
                    new()
                    {
                        ["Message"] = "Permanently delete this lease?",
                        ["SubMessage"] = "Payment history will also be removed.",
                        ["ConfirmText"] = "Delete"
                    },
                    new DialogOptions { Width = "420px" });

                if (ok is not true)
                    return;

                var done = await LeaseService.DeleteLeaseAsync(l.Id);

                NotificationService.Notify(done ? NotificationSeverity.Success : NotificationSeverity.Error, done ? "Deleted" : "Error", done ? "Lease deleted." : "Delete failed.");

                if (done)
                    await Load();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Failed to Confirm Delete",
                    Duration = 5000
                });
            }
        }

        #endregion
    }
}
