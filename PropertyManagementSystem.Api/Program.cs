using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PropertyManagementSystem.Application.Extensions;
using PropertyManagementSystem.Infrastructure.Extensions;
using PropertyManagementSystem.Infrastructure.Scheduling;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CONTROLLERS + SWAGGER
// =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================
// AUTH (JWT)
// =====================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// =====================
// CORS (Blazor UI)
// =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("UI", policy =>
    {
        policy.WithOrigins("https://localhost:7294")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =====================
// APPLICATION + INFRA
// =====================
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// ❌ REMOVE SignalR for now
// builder.Services.AddSignalR();

// =====================
// HANGFIRE
// =====================
builder.Services.AddHangfireServices(builder.Configuration);

var app = builder.Build();

// =====================
// PIPELINE
// =====================
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("UI");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

// =====================
// ROUTES
// =====================
app.MapControllers();

// ❌ REMOVE SignalR hubs
// app.MapHub<ChatHub>("/hubs/chat");
// app.MapHub<NotificationHub>("/hubs/notification");

// =====================
// HANGFIRE JOBS
// =====================
using (var scope = app.Services.CreateScope())
{
    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    HangfireJobs.Register(recurring);
}

app.Run();

//using Hangfire;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using PropertyManagementSystem.Application.Extensions;
//using PropertyManagementSystem.Infrastructure.Extensions;
//using PropertyManagementSystem.Infrastructure.Scheduling;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// =====================
//// Controllers
//// =====================
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// =====================
//// AUTH (JWT ONLY)
//// =====================
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

//builder.Services.AddAuthorization();

//// =====================
//// CORS (for Blazor UI)
//// =====================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("UI", policy =>
//    {
//        policy.WithOrigins("https://localhost:7294")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// =====================
//// APPLICATION + INFRA
//// =====================
//builder.Services.AddApplicationServices(builder.Configuration);
//builder.Services.AddInfrastructure(builder.Configuration);

//// =====================
//// HANGFIRE
//// =====================
//builder.Services.AddHangfireServices(builder.Configuration);

//var app = builder.Build();

//// =====================
//// PIPELINE
//// =====================
//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

//app.UseCors("UI");

//app.UseAuthentication();
//app.UseAuthorization();

//// =====================
//// HANGFIRE DASHBOARD
//// =====================
//app.UseHangfireDashboard("/hangfire");

//// =====================
//// ROUTES
//// =====================
//app.MapControllers();

//// =====================
//// HANGFIRE JOB REGISTRATION
//// =====================
//using (var scope = app.Services.CreateScope())
//{
//    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
//    HangfireJobs.Register(recurring);
//}

//app.Run();


//using Hangfire;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using PropertyManagementSystem.Application.Extensions;
//using PropertyManagementSystem.Infrastructure.Extensions;
//using PropertyManagementSystem.Infrastructure.Scheduling;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// =====================
//// Controllers
//// =====================
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// =====================
//// AUTH (JWT ONLY)
//// =====================
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

//builder.Services.AddAuthorization();

//// =====================
//// CORS (for Blazor UI)
//// =====================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("UI", policy =>
//    {
//        policy.WithOrigins("https://localhost:7294")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// =====================
//// APPLICATION + INFRA
//// =====================
////builder.Services.AddApplicationServices();
//builder.Services.AddApplicationServices(builder.Configuration);
//builder.Services.AddInfrastructure(builder.Configuration);

//// =====================
//// SIGNALR
//// =====================
//builder.Services.AddSignalR();

//// =====================
//// HANGFIRE (REGISTER SERVICES)
//// =====================
//builder.Services.AddHangfireServices(builder.Configuration);

//var app = builder.Build();

//// =====================
//// PIPELINE
//// =====================
//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

//app.UseCors("UI");

//app.UseAuthentication();
//app.UseAuthorization();

//// =====================
//// HANGFIRE DASHBOARD (optional but recommended)
//// =====================
//app.UseHangfireDashboard("/hangfire");

//// =====================
//// ROUTES
//// =====================
//app.MapControllers();

//// SignalR hubs
//app.MapHub<ChatHub>("/hubs/chat");
//app.MapHub<NotificationHub>("/hubs/notification");

//// =====================
//// HANGFIRE JOB REGISTRATION (IMPORTANT)
//// =====================
//using (var scope = app.Services.CreateScope())
//{
//    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

//    HangfireJobs.Register(recurring);
//}

//app.Run();