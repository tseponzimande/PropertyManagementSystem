namespace PropertyManagementSystem.UI.Components.Pages.Audit
{
    public partial class AuditPage
    {
        #region Dependencies

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private IAuditService AuditService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        #endregion

        #region Fields

        private List<AuditLogDto> logs { get; set; } = new();

        private List<UserDto> users { get; set; } = new();

        private bool IsLoading = false;

        private string? error = null;

        private string mode = "All";

        private Guid selectedUserId;

        private string actionFilter = string.Empty;

        private string entityName = "Property";

        private string entityIdStr = string.Empty;

        private DateTime dateRangeStart = DateTime.Today.AddMonths(-1);

        private DateTime dateRangeEnd = DateTime.Today;

        private List<string> modes = new() { "All", "User", "Action", "Entity", "DateRange" };

        private List<string> entityNames = new() { "Property", "Unit", "Lease", "Payment", "MaintenanceRequest", "User" };

        #endregion

        #region LifeCycle Methods

        protected override async Task OnInitializedAsync()
        {
            users = (await UserService.GetAllUsersAsync()).ToList();

            await Search();
        }

        #endregion

        #region Methods

        private async Task Search()
        {
            IsLoading = true;

            error = null;

            try
            {
                logs = mode switch
                {
                    "User" => (await AuditService.GetAuditLogsByUserAsync(selectedUserId)).ToList(),
                    "Action" => (await AuditService.GetAuditLogsByActionAsync(actionFilter)).ToList(),
                    "Entity" => Guid.TryParse(entityIdStr, out var eid) ? (await AuditService.GetAuditLogsByEntityAsync(entityName, eid)).ToList() :
                    throw new Exception("Invalid Id"),
                    "DateRange" => (await AuditService.GetAuditLogsByDateRangeAsync(dateRangeStart, dateRangeEnd.AddDays(1))).ToList(),
                    _ => (await AuditService.GetAuditLogsAsync(1, 200)).ToList(),
                };
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Please Try Again later !!!",
                    Duration = 6000
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ShowDiff(AuditLogDto dto)
        {
            try
            {

                await DialogService.OpenAsync<AuditDiffDialog>(
                    "Change Details",
                    new Dictionary<string, object>
                    {
                        ["Logs"] = logs
                    },
                    new DialogOptions { Width = "400px", CloseDialogOnOverlayClick = true });
            }
            catch (Exception ex)
            {
                error = $"Error occured :{ex.Message}";

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry Try Again later",
                    Duration = 5000
                });
            }
        }

        #endregion
    }
}
