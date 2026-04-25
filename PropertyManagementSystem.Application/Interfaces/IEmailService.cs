namespace PropertyManagementSystem.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendPaymentConfirmationAsync(string toEmail, string userName, decimal amount, DateTime paymentDate);
        Task<bool> SendLeaseExpiryReminderAsync(string toEmail, string userName, DateTime expiryDate);
        Task<bool> SendMaintenanceUpdateAsync(string toEmail, string userName, string requestTitle, string status);
    }
}
