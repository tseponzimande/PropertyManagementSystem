namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class PropertyFormDialog
    {

        [Inject]
        private IPropertyService PropertyService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService Notification { get; set; } = null!;


        [Parameter]
        public PropertyDto Model { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }


        private bool _busy = false;
        private string? _error = null;

        private readonly List<string> _categories = new() { "Residential", "Commercial", "Industrial", "Mixed Use" };
        private readonly List<string> _statuses = new() { "Approved", "Pending", "Reject" };

        private async Task Save(PropertyDto dto)
        {
            _busy = true;

            _error = null;

            try
            {
                if (IsEdit)
                {
                    var ok = await PropertyService.UpdatePropertyAsync(dto);

                    if (!ok)
                    {
                        _error = "Update failed. The property may no longer exist.";
                        return;
                    }
                    Notification.Notify(NotificationSeverity.Success, "Saved", "Property updated.");
                }
                else
                {
                    var result = await PropertyService.CreatePropertyAsync(dto);

                    if (result is null)
                    {
                        _error = "Create failed. Please check all fields.";
                        return;
                    }
                    Notification.Notify(NotificationSeverity.Success, "Created", "Property submitted for approval.");
                }
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            {
                _busy = false;
            }
        }
    }
}
