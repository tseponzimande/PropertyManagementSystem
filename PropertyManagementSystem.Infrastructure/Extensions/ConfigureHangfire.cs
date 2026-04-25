using PropertyManagementSystem.Infrastructure.Jobs;

namespace PropertyManagementSystem.Infrastructure.Extensions
{
    public static class ConfigureHangfire
    {
        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Hangfire

            var connectionString = configuration.GetConnectionString("Database");
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();


            services.AddScoped<MaintenanceFollowUpJob>();

            services.AddScoped<RentReminderJob>();

            services.AddScoped<LeaseExpiryJob>();

            return services;

            #endregion
        }
    }
}
