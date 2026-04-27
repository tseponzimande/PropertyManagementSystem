namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class UserFormDialog
    {
        #region Dependencies

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public UserDto Model { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        #endregion

        #region Fields

        private bool busy = false;
        private string? error = null;
        private string password = string.Empty;

        private readonly List<string> roles = new() { "Admin", "Owner", "Tenant" };

        #endregion

        #region Methods

        private async Task Save(UserDto dto)
        {
            busy = true;

            error = null;

            try
            {
                if (IsEdit)
                {
                    await UserService.UpdateUserAsync(dto);
                    NotificationService.Notify(NotificationSeverity.Success, "Saved", $"'{dto.Name}' updated.");
                }
                else
                {
                    var result = await UserService.CreateUserAsync(dto, password, dto.Role);

                    if (result is null)
                    {
                        error = "Create failed. Email may already be in use.";
                        return;
                    }

                    NotificationService.Notify(NotificationSeverity.Success, "Created", $"'{dto.Name}' created.");
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