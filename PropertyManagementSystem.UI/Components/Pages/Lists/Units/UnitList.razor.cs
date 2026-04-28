namespace PropertyManagementSystem.UI.Components.Pages.Lists.Units
{
    public partial class UnitList
    {
        #region Dependencies

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private IPropertyService PropertyService { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields

        private List<UnitDto> all = new();

        private List<UnitDto> rows = new();

        private List<PropertyDto> properties = new();

        private bool loading = true;

        private string? error = null;

        private string statusFilter = string.Empty;

        private Guid? propFilter;


        private readonly List<string> _unitStatuses = new() { "Available", "Occupied" };

        protected override async Task OnInitializedAsync() => await Load();

        #endregion

        #region LIfeCycle Methods
        private async Task Load()
        {
            loading = true; 
            
            error = null;
            try
            {
                all = (await UnitService.GetAllUnitsAsync()).ToList();
                properties = (await PropertyService.GetAllPropertiesAsync()).ToList();
                ApplyFilter(null);
            }
            catch 
            (Exception ex) 
            { 
                error = "Failed to load units.";
                Console.Error.WriteLine(ex);
            }
            finally 
            {
                loading = false; 
            }
        }

        #endregion

        #region Methods

        private void ApplyFilter(object? _)
        {
            rows = all
                .Where(u => string.IsNullOrEmpty(statusFilter) || u.Status == statusFilter)
                .Where(u => propFilter is null || u.PropertyId == propFilter)
                .ToList();
        }

        private async Task UpdateStatus(UnitDto u, string newStatus)
        {
            try
            {
                var ok = await UnitService.UpdateUnitStatusAsync(u.Id, newStatus);
                if (ok)
                {
                    u.Status = newStatus;
                    NotificationService.Notify(NotificationSeverity.Success, "Updated", $"Unit {u.UnitNumber} → {newStatus}");
                }
                else NotificationService.Notify(NotificationSeverity.Error, "Error", "Status update failed.");
            }
            catch 
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Status update failed.");
            }
        }

        private async Task OpenAdd()
        {
            try
            {
                var dto = new UnitDto
                {
                    Status = UnitEnum.Available.ToString()
                };

                var ok = await DialogService.OpenAsync<UnitFormDialog>(
                    "Add Unit",
                    new()
                    {
                        ["Model"] = dto,
                        ["IsEdit"] = false,
                        ["Properties"] = properties
                    },
                    new DialogOptions { Width = "440px", Draggable = true });

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
                    Detail = "Sorry Please try Again !!!",
                    Duration = 5000
                });
            }
        }

        private async Task OpenEdit(UnitDto u)
        {
            try
            {
                var clone = new UnitDto
                {
                    Id = u.Id,
                    UnitNumber = u.UnitNumber,
                    RentAmount = u.RentAmount,
                    Status = u.Status,
                    PropertyId = u.PropertyId
                };

                var ok = await DialogService.OpenAsync<UnitFormDialog>(
                    "Edit Unit",
                    new()
                    {
                        ["Model"] = clone,
                        ["IsEdit"] = true,
                        ["Properties"] = properties
                    },
                    new DialogOptions { Width = "440px", Draggable = true });

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
                    Detail = "Sorry Please try Again !!!",
                    Duration = 5000
                });
            }
        }

        private async Task ConfirmDelete(UnitDto u)
        {
            try
            {
                var ok = await DialogService.OpenAsync<ConfirmDialog>(
                    "Delete Unit",
                    new()
                    {
                        ["Message"] = $"Delete Unit '{u.UnitNumber}'?",
                        ["SubMessage"] = "Associated leases may be affected.",
                        ["ConfirmText"] = "Delete"
                    },
                    new DialogOptions { Width = "400px" });

                if (ok is not true)
                    return;

                var done = await UnitService.DeleteUnitAsync(u.Id);

                NotificationService.Notify(done ? NotificationSeverity.Success : NotificationSeverity.Error, done ? "Deleted" : "Error", done ? $"Unit {u.UnitNumber} removed." : "Delete failed.");
                await Load();
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Please try Again !!!",
                    Duration = 5000
                });
            }
        }

        #endregion
    }
}