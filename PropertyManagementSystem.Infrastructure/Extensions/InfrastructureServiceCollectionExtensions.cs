namespace PropertyManagementSystem.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            #region DbContext

            var conn = config.GetConnectionString("Database");

            if (string.IsNullOrWhiteSpace(conn))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            services.AddDbContext<PropertyManagementDbContext>(options =>
            {
                options.UseSqlServer(conn);
            });

            #endregion

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IPropertyRepository, PropertyRepository>();

            services.AddScoped<IUnitRepository, UnitRepository>();

            services.AddScoped<ILeaseRepository, LeaseRepository>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();

            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<IAuditLogRepository, AuditLogRepository>();


            #endregion

            #region Unit of Work

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion

            return services;
        }
    }
}
