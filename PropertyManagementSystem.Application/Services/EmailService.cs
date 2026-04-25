namespace PropertyManagementSystem.Application.Services
{
    public class EmailService : IEmailService
    {
        #region Dependencies

        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableTls;

        #endregion

        #region Constructor

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;

            _logger = logger;

            _smtpHost = _configuration["SMTP:Host"] ?? "";

            _smtpPort = int.Parse(_configuration["SMTP:Port"] ?? "587");

            _smtpUsername = _configuration["SMTP:Username"] ?? "";

            _smtpPassword = _configuration["SMTP:Password"] ?? "";

            _enableTls = bool.Parse(_configuration["SMTP:TLS"] ?? "true");
        }

        #endregion

        #region Methods

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_smtpUsername);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
                smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                smtpClient.EnableSsl = _enableTls;

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var subject = "Welcome to Real Estate Application";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2563eb;'>Welcome to Real Estate Application!</h2>
                        <p>Dear {userName},</p>
                        <p>Thank you for registering with our Real Estate Application.</p>
                        <p>You can now:</p>
                        <ul>
                            <li>Manage your properties</li>
                            <li>Track lease agreements</li>
                            <li>Monitor payments</li>
                            <li>Submit maintenance requests</li>
                        </ul>
                        <p>If you have any questions, feel free to contact our support team.</p>
                        <br>
                        <p>Best regards,<br>Real Estate Team</p>
                    </div>
                </body>
                </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> SendPaymentConfirmationAsync(string toEmail, string userName, decimal amount, DateTime paymentDate)
        {
            try
            {
                var subject = "Payment Confirmation - Real Estate Application";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #10b981;'>Payment Received</h2>
                        <p>Dear {userName},</p>
                        <p>We have successfully received your payment.</p>
                        <div style='background-color: #f3f4f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>Amount:</strong> ${amount:N2}</p>
                            <p><strong>Date:</strong> {paymentDate:MMMM dd, yyyy}</p>
                        </div>
                        <p>Thank you for your timely payment!</p>
                        <br>
                        <p>Best regards,<br>Real Estate Team</p>
                    </div>
                </body>
                </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> SendLeaseExpiryReminderAsync(string toEmail, string userName, DateTime expiryDate)
        {
            try
            {
                var daysUntilExpiry = (expiryDate - DateTime.UtcNow).Days;
                var subject = "Lease Expiry Reminder - Real Estate Application";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #f59e0b;'>Lease Expiry Reminder</h2>
                        <p>Dear {userName},</p>
                        <p>This is a reminder that your lease agreement will expire in <strong>{daysUntilExpiry} days</strong>.</p>
                        <div style='background-color: #fef3c7; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>Expiry Date:</strong> {expiryDate:MMMM dd, yyyy}</p>
                        </div>
                        <p>Please contact your landlord to discuss renewal options.</p>
                        <br>
                        <p>Best regards,<br>Real Estate Team</p>
                    </div>
                </body>
                </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        public async Task<bool> SendMaintenanceUpdateAsync(string toEmail, string userName, string requestTitle, string status)
        {
            try
            {
                var subject = "Maintenance Request Update - Real Estate Application";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2563eb;'>Maintenance Request Update</h2>
                        <p>Dear {userName},</p>
                        <p>Your maintenance request has been updated.</p>
                        <div style='background-color: #dbeafe; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>Request:</strong> {requestTitle}</p>
                            <p><strong>Status:</strong> {status}</p>
                        </div>
                        <p>Thank you for your patience.</p>
                        <br>
                        <p>Best regards,<br>Real Estate Team</p>
                    </div>
                </body>
                </html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed");
                return false;
            }
        }

        #endregion
    }
}