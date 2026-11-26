using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Infrastructure.Persistence;
using NexaWork.Infrastructure.Services;

namespace NexaWork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. EF Core Database Configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString) && !connectionString.Contains("Host=localhost"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        else
        {
            // Fallback to SQLite or InMemory for lightweight execution / testing if Postgres connection is unavailable
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString ?? "Host=localhost;Database=nexawork_db;Username=postgres;Password=postgres"));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // 2. HTTP Context & Current User
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 3. JWT Token Generator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // 4. Cache & Email & SignalR Service
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<INotificationHubService, NotificationHubService>();

        // 5. JWT Authentication Setup
        var jwtSecret = configuration["JwtSettings:Secret"] ?? "EnterpriseNexaWorkSuperSecretKeyForJwt20261125VeryLongKey!";
        var jwtIssuer = configuration["JwtSettings:Issuer"] ?? "NexaWork";
        var jwtAudience = configuration["JwtSettings:Audience"] ?? "NexaWorkClient";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // SignalR token support from query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return System.Threading.Tasks.Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
