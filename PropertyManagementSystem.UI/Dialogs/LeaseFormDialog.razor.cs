namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class LeaseFormDialog
    {
        #region Dependencies

        [Inject]
        private ILeaseService LeaseService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public LeaseDto Model { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public List<UnitDto> Units { get; set; } = new();

        [Parameter]
        public List<UserDto> Users { get; set; } = new();

        #endregion

        #region Fields

        private bool busy = false;
        private string? error = null;

        private List<UserDto> tenants => Users.Where(u => u.Role == RoleEnum.Tenant.ToString()).ToList();

        private readonly List<string> _statuses = new() { "Active", "Expired", "Terminated" };

        #endregion

        #region Methods

        private async Task Save(LeaseDto dto)
        {
            busy = true;

            error = null;

            try
            {
                if (dto.EndDate <= dto.StartDate)
                {
                    error = "End date must be after start date.";
                    return;
                }

                if (IsEdit)
                {
                    var ok = await LeaseService.UpdateLeaseAsync(dto);

                    if (!ok)
                    {
                        error = "Update failed.";
                        return;
                    }
                    NotificationService.Notify(NotificationSeverity.Success, "Saved", "Lease updated.");
                }
                else
                {                
                    dto.Status = LeaseStatus.Active.ToString();

                    var result = await LeaseService.CreateLeaseAsync(dto);

                    if (result is null)
                    {
                        error = "Create failed.";
                        return;
                    }
                    NotificationService.Notify(NotificationSeverity.Success, "Created", "Lease created.");
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
