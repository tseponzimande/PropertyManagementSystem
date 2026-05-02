namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class PageHeader
    {
        #region Dependencies

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Icon { get; set; } = "circle";

        [Parameter]
        public string Subtitle { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment? Actions { get; set; }

        #endregion
    }
}
