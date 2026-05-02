namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class DataLoadWrapper
    {
        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter] 
        public string? ErrorMessage { get; set; }

        [Parameter] 
        public bool HasData { get; set; }

        [Parameter] 
        public string EmptyMessage { get; set; } = "No data found.";

        [Parameter] 
        public string EmptyIcon { get; set; } = "inbox";

        [Parameter]
        public RenderFragment? LoadingContent { get; set; }

        [Parameter]
        public RenderFragment? DataContent { get; set; }

        [Parameter] 
        public RenderFragment? EmptyContent { get; set; }
    }
}
