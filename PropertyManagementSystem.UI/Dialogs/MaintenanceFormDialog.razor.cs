namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class MaintenanceFormDialog
    {
        #region Dependencies

        [Inject]
        private IMaintenanceService MaintenanceService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        #endregion

        #region Parameters

        [Parameter]
        public MaintenanceRequestDto model { get; set; } = new();

        [Parameter]
        public List<UnitDto> Units { get; set; } = null!;

        #endregion

        #region Fields

        private bool isLoading = false;

        private string? error = null!;

        #endregion

        #region Methods

        private async Task Save(MaintenanceRequestDto dto)
        {
            isLoading = true;

            error = null;

            try
            {
                dto.Status = MaintenanceRequestEnum.Open.ToString();

                var res = await MaintenanceService.CreateRequestAsync(dto);

                if(res is null)
                {
                    error = "Submission failed. Please try again.";
                    return;
                }

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Submited",
                    Detail = $" : {dto.Title}",
                    Duration = 5000
                });

                DialogService.Close();
            }
            catch(Exception ex)
            {
                error = $"Error Occured: {ex.Message}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = error,
                    Detail = "Sorry please try again later !!!",
                    Duration = 5000
                });
            }
            finally
            {
                isLoading = false; 
            }
        }



        #endregion
    }
}
