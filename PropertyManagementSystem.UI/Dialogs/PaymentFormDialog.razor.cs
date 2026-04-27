namespace PropertyManagementSystem.UI.Dialogs
{
    public partial class PaymentFormDialog
    {
        #region Dependencies

        [Inject]
        private IPaymentService PaymentService { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        #endregion

        #region Fields

        private bool busy = false;

        private string? error = null;

        #endregion

        #region Parameters

        [Parameter]
        public PaymentDto Model { get; set; } = new();

        #endregion

        #region Methods

        private async Task Save(PaymentDto dto)
        {
            busy = true;

            error = null;

            try
            {
                var result = await PaymentService.CreatePaymentAsync(dto);

                if (result is null)
                {
                    error = "Payment could not be recorded.";
                    return;
                }

                NotificationService.Notify(NotificationSeverity.Success, "Recorded", $"{dto.Amount:C0} payment recorded.");

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
