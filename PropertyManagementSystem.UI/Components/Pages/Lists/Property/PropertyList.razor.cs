namespace PropertyManagementSystem.UI.Components.Pages.Lists.Property
{
    public partial class PropertyList
    {
        [Inject]
        private IPropertyService PropertyService { get; set; } = null!;

        [Inject]
        private AuthStateService AuthState { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private List<PropertyDto> all = new();
        private List<PropertyDto> rows = new();
        private bool loading = true;
        private string? error = null;
        private string statusFilter = string.Empty;
        private Guid busyId = Guid.Empty;
        private Guid currentUserId;
        private bool isAdmin;

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();
            currentUserId = state.User.GetUserId();
            isAdmin = state.User.IsAdmin();
            await Load();
        }

        private async Task Load()
        {
            loading = true; error = null;
            try
            {
                all = isAdmin ? (await PropertyService.GetAllPropertiesAsync()).ToList() : (await PropertyService.GetPropertiesByOwnerAsync(currentUserId)).ToList();
                ApplyFilter(statusFilter);
            }
            catch (Exception ex)
            {
                error = "Failed to load properties.";
                Console.Error.WriteLine(ex);
            }
            finally
            {
                loading = false;
            }
        }

        private void ApplyFilter(string? v)
        {
            statusFilter = v ?? string.Empty;

            rows = string.IsNullOrEmpty(statusFilter) ? all.ToList() : all.Where(p => p.Status == statusFilter).ToList();
        }

        private async Task OpenAdd()
        {
            var dto = new PropertyDto { OwnerId = currentUserId };

            var ok = await DialogService.OpenAsync<PropertyFormDialog>(
                "Add Property",
                new() { ["Model"] = dto, ["IsEdit"] = false },
                new DialogOptions { Width = "480px" });

            if (ok is true)
                await Load();
        }

        private async Task OpenEdit(PropertyDto p)
        {
            var clone = new PropertyDto { Id = p.Id, Address = p.Address, Category = p.Category, Status = p.Status, OwnerId = p.OwnerId };

            var ok = await DialogService.OpenAsync<PropertyFormDialog>(
                "Edit Property",
                new() { ["Model"] = clone, ["IsEdit"] = true },
                new DialogOptions { Width = "480px" });

            if (ok is true)
                await Load();
        }

        private async Task ConfirmDelete(PropertyDto p)
        {
            var ok = await DialogService.OpenAsync<ConfirmDialog>(
                "Delete Property",
                new() { ["Message"] = $"Delete '{p.Address}'?", ["SubMessage"] = "All associated units will also be removed.", ["ConfirmText"] = "Delete" },
                new DialogOptions { Width = "420px" });

            if (ok is not true)
                return;

            try
            {
                var done = await PropertyService.DeletePropertyAsync(p.Id);

                NotificationService.Notify(done ? NotificationSeverity.Success : NotificationSeverity.Error, done ? "Deleted" : "Error", done ? $"'{p.Address}' removed." : "Delete failed.");
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Delete failed.");
            }
            await Load();
        }

        private async Task Approve(PropertyDto p)
        {
            busyId = p.Id;

            try
            {
                var ok = await PropertyService.ApprovePropertyAsync(p.Id);

                NotificationService.Notify(ok ? NotificationSeverity.Success : NotificationSeverity.Error,
                ok ? "Approved" : "Error", ok ? $"'{p.Address}' approved." : "Failed.");

                if (ok)
                    await Load();
            }
            finally { busyId = Guid.Empty; }
        }

        private async Task Reject(PropertyDto p)
        {
            busyId = p.Id;

            try
            {
                var ok = await PropertyService.RejectPropertyAsync(p.Id);

                NotificationService.Notify(ok ? NotificationSeverity.Warning : NotificationSeverity.Error,
                    ok ? "Rejected" : "Error", ok ? $"'{p.Address}' rejected." : "Failed.");

                if (ok)
                    await Load();
            }
            finally
            {
                busyId = Guid.Empty;
            }
        }
    }
}
