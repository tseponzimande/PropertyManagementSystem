namespace PropertyManagementSystem.UI.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private AuthStateService AuthStateService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;


        protected override async Task OnInitializedAsync()
        {
            var user = await AuthStateService.GetClaimsPrincipalAsync();

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            Navigation.NavigateTo("/dashboard");
        }
    }
}