using PropertyManagementSystem.Domain.Entities;

namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class UnitFormDialog
    {
        #region Dependencies

        [Inject]
        private IUnitService UnitService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public UnitDto Model { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public List<PropertyDto> Properties { get; set; } = new();

        #endregion

        #region Fields

        private bool busy = false;

        private string? error = null;

        private readonly List<string> statuses = new() { "Available", "Occupied" };

        #endregion


        #region Methods

        private async Task Save(UnitDto dto)
        {
            busy = true;

            error = null;

            try
            {
                if (IsEdit)
                {
                    var ok = await UnitService.UpdateUnitAsync(dto);

                    if (!ok)
                    {
                        error = "Update failed.";
                        return;
                    }
                    NotificationService.Notify(NotificationSeverity.Success, "Saved", "Unit updated.");
                }
                else
                {
                    var result = await UnitService.CreateUnitAsync(dto);

                    if (result is null)
                    {
                        error = "Create failed.";
                        return;
                    }
                    NotificationService.Notify(NotificationSeverity.Success, "Created", $"Unit {dto.UnitNumber} created.");
                }
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                busy = false;
            }
        }

        #endregion

    }
}
