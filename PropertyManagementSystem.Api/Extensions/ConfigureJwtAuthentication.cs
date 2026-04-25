namespace PropertyManagementSystem.Api.Extensions
{
    public static class ConfigureJwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Key"] ??
                throw new InvalidOperationException("JWT Secret Key not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                // Hub path check removed — SignalR hubs are not active
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
                options.AddPolicy("TenantOnly", policy => policy.RequireRole("Tenant"));
                options.AddPolicy("OwnerOrAdmin", policy => policy.RequireRole("Owner", "Admin"));
                options.AddPolicy("TenantOrOwner", policy => policy.RequireRole("Tenant", "Owner"));
            });

            return services;
        }
    }
}


//namespace PropertyManagementSystem.Api.Extensions
//{
//    public static class ConfigureJwtAuthentication
//    {
//        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
//        {
//            #region JwtAuthentication

//            var secretKey = configuration["Jwt:Key"] ??
//                throw new InvalidOperationException("JWT Secret Key not configured");

//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            .AddJwtBearer(options =>
//            {
//                options.SaveToken = true;
//                options.RequireHttpsMetadata = false;
//                options.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    ValidateIssuerSigningKey = true,
//                    ValidIssuer = configuration["Jwt:Issuer"],
//                    ValidAudience = configuration["Jwt:Audience"],
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//                    ClockSkew = TimeSpan.Zero 
//                };


//                options.Events = new JwtBearerEvents
//                {
//                    OnMessageReceived = context =>
//                    {
//                        var accessToken = context.Request.Query["access_token"];
//                        var path = context.HttpContext.Request.Path;

//                        if (!string.IsNullOrEmpty(accessToken) &&
//                            (path.StartsWithSegments("/hubs/chat") ||
//                             path.StartsWithSegments("/hubs/notification")))
//                        {
//                            context.Token = accessToken;
//                        }
//                        return Task.CompletedTask;
//                    }
//                };
//            });

//            services.AddAuthorization(options =>
//            {

//                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//                options.AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
//                options.AddPolicy("TenantOnly", policy => policy.RequireRole("Tenant"));
//                options.AddPolicy("OwnerOrAdmin", policy => policy.RequireRole("Owner", "Admin"));
//                options.AddPolicy("TenantOrOwner", policy => policy.RequireRole("Tenant", "Owner"));
//            });

//            return services;

//            #endregion
//        }
//    }
//}