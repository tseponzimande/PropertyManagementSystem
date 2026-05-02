namespace PropertyManagementSystem.UI.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        private AuthStateService AuthStateService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private bool sidebar1Expanded = true;

        private async Task Logout()
        {
            await AuthStateService.LogoutAsync();
            Navigation.NavigateTo("/login", true);
        }
    }
}