using Azure.Core;
using PropertyManagementSystem.Application.DTOs.AuditLog;

namespace PropertyManagementSystem.UI.Components.Pages.Properties
{
    public partial class PropertyDetail
    {
        #region Depedencies

        [Inject]
        private IPropertyService PropertyService { get; set; } = null!;

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private IAuditService AuditService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public Guid Id { get; set; }

        #endregion

        #region Fields

        private PropertyDto? property = null;

        private List<UnitDto> units = new();

        private List<AuditLogDto> audit = new();

        private bool loading = true;

        private bool loadingAudit = false;

        private string? error = null;

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            try
            {
                property = await PropertyService.GetPropertyByIdAsync(Id);
                if (property is not null)
                {
                    units = (await UnitService.GetUnitsByPropertyAsync(Id)).ToList();
                    await LoadAudit();
                }
            }
            finally
            {
                loading = false;
            }
        }

        #endregion

        #region Methods

        private async Task LoadAudit()
        {
            loadingAudit = true;

            try
            {
                audit = (await AuditService.GetAuditLogsByEntityAsync("Property", Id)).ToList();
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Could Not Load Audit",
                    Duration = 5000
                });
            }
            finally
            {
                loadingAudit = false;
            }
        }

        private async Task OpenEdit()
        {
            try
            {
                if (property is null)
                    return;

                var clone = new PropertyDto
                {
                    Id = property.Id,
                    Address = property.Address,
                    Category = property.Category,
                    Status = property.Status,
                    OwnerId = property.OwnerId
                };
                var ok = await DialogService.OpenAsync<PropertyFormDialog>(
                "Edit Property",
                    new()
                    {
                        ["Model"] = clone,
                        ["IsEdit"] = true
                    },
                    new DialogOptions { Width = "480px" });

                if (ok is true)
                    property = await PropertyService.GetPropertyByIdAsync(Id);
            }
            catch(Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Could Not Open dialog",
                    Duration = 5000
                });
            }
        }

        private async Task OpenAddUnit()
        {
            try
            {
                var dto = new UnitDto
                {
                    PropertyId = Id,
                    Status = "Available"
                };

                var ok = await DialogService.OpenAsync<UnitFormDialog>(
                "Add Unit",
                    new()
                    {
                        ["Model"] = dto,
                        ["IsEdit"] = false,
                        ["Properties"] = new List<PropertyDto> { property! }
                    },
                    new DialogOptions { Width = "440px" });

                if (ok is true)
                    units = (await UnitService.GetUnitsByPropertyAsync(Id)).ToList();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Could Not Load Audit",
                    Duration = 5000
                });
            }
        }
    }

    #endregion
}

