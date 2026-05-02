namespace PropertyManagementSystem.UI.Components.Shared
{
    public partial class ConfirmActionButton
    {
        [Parameter]
        public string Label { get; set; } = "Delete";

        [Parameter] 
        public string Icon { get; set; } = "delete";

        [Parameter]
        public ButtonStyle Style { get; set; } = ButtonStyle.Danger;

        [Parameter]
        public ButtonSize Size { get; set; } = ButtonSize.Small;

        [Parameter] 
        public Variant Variant { get; set; } = Variant.Text;

        [Parameter]
        public string? ConfirmText { get; set; } = "Are you sure?";

        [Parameter] 
        public bool Disabled { get; set; }

        [Parameter] 
        public EventCallback OnConfirmed { get; set; }

        private bool confirming = false;
        private bool busy = false;

        private async Task Confirm()
        {
            busy = true;

            try 
            {
                await OnConfirmed.InvokeAsync();
            }
            finally
            { 
                busy = false;
                confirming = false; 
            }
        }
    }
}
