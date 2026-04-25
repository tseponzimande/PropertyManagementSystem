namespace PropertyManagementSystem.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region AutoMapper

            services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

            #endregion

            #region Jwt Settings

            services.AddOptions<JwtSettings>()
                .BindConfiguration("Jwt")
                .Validate(s => !string.IsNullOrWhiteSpace(s.Key), "JWT Key is required")
                .Validate(s => !string.IsNullOrWhiteSpace(s.Issuer), "JWT Issuer is required")
                .Validate(s => !string.IsNullOrWhiteSpace(s.Audience), "JWT Audience is required")
                .ValidateOnStart();

            #endregion

            #region register application services

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ILeaseService, LeaseService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUserService, UserService>();

            #endregion

            return services;
        }
    }
}