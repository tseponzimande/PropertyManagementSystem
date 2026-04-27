namespace PropertyManagementSystem.UI.Components.Pages.Lists.Users
{
    public partial class UserList
    {
        #region Dependencies

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        #endregion

        #region Fields

        private List<UserDto> all = new();

        private List<UserDto> rows = new();

        private bool loading = true;

        private string? error = null;

        private string roleFilter = string.Empty;

        #endregion

        #region LifeCyle Methods

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        #endregion

        #region Methods

        private async Task LoadAsync()
        {
            loading = true;

            error = null;

            try
            {
                all = (await UserService.GetAllUsersAsync()).ToList();
                ApplyFilter(null);
            }
            catch (Exception ex)
            {
                error = $"Failed to load users : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Could Not Load Sorry !!!",
                    Duration = 5000
                });
            }
            finally
            {
                loading = false;
            }
        }

        private void ApplyFilter(object? _)
        {
            rows = string.IsNullOrEmpty(roleFilter)
                ? all.ToList()
                : all.Where(u => u.Role != null && u.Role.ToString() == roleFilter).ToList();
        }

        //private void ApplyFilter(object? _)
        //{
        //    rows = string.IsNullOrEmpty(roleFilter) ? all.ToList() : all.Where(u => u.Role == roleFilter).ToList();
        //}

        private async Task OpenAdd()
        {
            try
            {
                var ok = await DialogService.OpenAsync<UserFormDialog>(
                "Add User",
                    new()
                    {
                        ["Model"] = new UserDto(),
                        ["IsEdit"] = false
                    },
                    new DialogOptions { Width = "460px", Draggable=true });

                if (ok is true)
                    await LoadAsync();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Could Not Open Dialog Sorry !!!",
                    Duration = 5000
                });
            }
        }

        private async Task OpenEdit(UserDto u)
        {
            try
            {
                var clone = new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                };
                var ok = await DialogService.OpenAsync<UserFormDialog>(
                "Edit User",
                    new()
                    {
                        ["Model"] = clone,
                        ["IsEdit"] = true
                    },

                    new DialogOptions { Width = "460px" });

                if (ok is true)
                    await LoadAsync();
            }
            catch (Exception ex)
            {
                error = $"Error Occured : {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Could Not Open Dialog Sorry !!!",
                    Duration = 5000
                });
            }
        }

        private async Task ConfirmDelete(UserDto u)
        {
            var ok = await DialogService.OpenAsync<ConfirmDialog>(
            "Delete User",
                new()
                {
                    ["Message"] = $"Delete '{u.Name}'?",
                    ["SubMessage"] = "This will permanently remove the user.",
                    ["ConfirmText"] = "Delete"
                },

                new DialogOptions { Width = "420px" });

            if (ok is not true)
                return;

            try
            {
                await UserService.DeleteUserAsync(u.Id);
                NotificationService.Notify(NotificationSeverity.Success, "Deleted", $"'{u.Name}' removed.");
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Delete failed.");
            }
            await LoadAsync();
        }

        #endregion
    }
}
