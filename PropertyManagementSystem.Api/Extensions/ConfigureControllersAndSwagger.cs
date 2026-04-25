namespace PropertyManagementSystem.Api.Extensions
{
    public static class ConfigureControllersAndSwagger
    {
        public static IServiceCollection AddControllersAndSwagger(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Property Management System API",
                    Version = "v1",
                    Description = "API for Property Management System"
                });

                // Add support for file uploads in Swagger
                c.OperationFilter<FileUploadOperationFilter>();

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerIfDevelopment(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Property Management System API V1");
                    c.RoutePrefix = "swagger";
                });
            }
            return app;
        }
    }

    // Custom operation filter to handle file uploads
    public class FileUploadOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(Microsoft.OpenApi.Models.OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            var formFileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.ModelMetadata?.ContainerType != null &&
                           p.ModelMetadata.ContainerType == typeof(IFormFile))
                .ToList();

            if (!formFileParams.Any())
            {
                // Check if any parameter is IFormFile
                var hasFormFile = context.ApiDescription.ParameterDescriptions
                    .Any(p => p.Type == typeof(IFormFile) ||
                             (p.Type.IsGenericType && p.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                              && p.Type.GetGenericArguments()[0] == typeof(IFormFile)));

                if (!hasFormFile)
                    return;
            }

            // Set the consumes type to multipart/form-data
            operation.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
            {
                Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
                {
                    ["multipart/form-data"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                    {
                        Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                        {
                            Type = "object",
                            Properties = context.ApiDescription.ParameterDescriptions
                                .ToDictionary(
                                    p => p.Name,
                                    p => p.Type == typeof(IFormFile)
                                        ? new Microsoft.OpenApi.Models.OpenApiSchema
                                        {
                                            Type = "string",
                                            Format = "binary"
                                        }
                                        : new Microsoft.OpenApi.Models.OpenApiSchema
                                        {
                                            Type = GetSchemaType(p.Type)
                                        }
                                ),
                            Required = context.ApiDescription.ParameterDescriptions
                                .Where(p => p.IsRequired)
                                .Select(p => p.Name)
                                .ToHashSet()
                        }
                    }
                }
            };
        }

        private string GetSchemaType(Type type)
        {
            if (type == typeof(string)) return "string";

            if (type == typeof(int) || type == typeof(long)) return "integer";

            if (type == typeof(bool)) return "boolean";

            if (type == typeof(Guid)) return "string";

            return "string";
        }
    }
}
