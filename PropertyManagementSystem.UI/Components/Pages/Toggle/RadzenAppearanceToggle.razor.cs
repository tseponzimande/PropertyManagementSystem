using Microsoft.AspNetCore.Components;

namespace PropertyManagementSystem.UI.Components.Pages.Toggle
{
    public partial class RadzenAppearanceToggle : RadzenComponent
    {
        [Inject]
        private ThemeService? ThemeService { get; set; }

        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        [Parameter]
        public ButtonStyle ButtonStyle { get; set; } = ButtonStyle.Base;

        [Parameter]
        public Shade ToggleShade { get; set; } = Shade.Default;

        [Parameter]
        public ButtonStyle ToggleButtonStyle { get; set; } = ButtonStyle.Base;

        [Parameter]
        public string? LightTheme { get; set; }

        [Parameter]
        public string? DarkTheme { get; set; }

        private string CurrentLightTheme => LightTheme ?? ThemeService?.Theme?.ToLowerInvariant() switch
        {
            "dark" => "default",
            "material-dark" => "material",
            "fluent-dark" => "fluent",
            "material3-dark" => "material3",
            "software-dark" => "software",
            "humanistic-dark" => "humanistic",
            "standard-dark" => "standard",
            _ => ThemeService?.Theme ?? string.Empty,
        };

        private string CurrentDarkTheme => DarkTheme ?? ThemeService?.Theme?.ToLowerInvariant() switch
        {
            "default" => "dark",
            "material" => "material-dark",
            "fluent" => "fluent-dark",
            "material3" => "material3-dark",
            "software" => "software-dark",
            "humanistic" => "humanistic-dark",
            "standard" => "standard-dark",
            _ => ThemeService?.Theme ?? string.Empty,
        };

        private bool value;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ThemeService != null)
            {
                ThemeService.ThemeChanged += OnThemeChanged;

                value = ThemeService.Theme != CurrentDarkTheme;
            }
        }

        private void OnThemeChanged()
        {
            if (ThemeService != null)
            {
                value = ThemeService.Theme != CurrentDarkTheme;
            }

            StateHasChanged();
        }

        void OnChange(bool value)
        {
            ThemeService?.SetTheme(value ? CurrentLightTheme : CurrentDarkTheme);
        }

        private string Icon => value ? "dark_mode" : "light_mode";

        public override void Dispose()
        {
            base.Dispose();

            if (ThemeService != null)
            {
                ThemeService.ThemeChanged -= OnThemeChanged;
            }

            GC.SuppressFinalize(this);
        }
    }
}
