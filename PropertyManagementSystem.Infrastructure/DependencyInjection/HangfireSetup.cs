namespace PropertyManagementSystem.Infrastructure.DependencyInjection
{
    public static class HangfireSetup
    {
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(
                    config.GetConnectionString("DefaultConnection"));
            });

            services.AddHangfireServer();

            return services;
        }
    }
}