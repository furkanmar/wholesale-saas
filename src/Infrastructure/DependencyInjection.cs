using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Wholesale.Application.Common.Interfaces;
using Wholesale.Infrastructure.Data;
using Wholesale.Infrastructure.Data.Interceptors;
using Wholesale.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        // Tenant context — request scope'unda yaşar
        builder.Services.AddScoped<TenantContext>();
        builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

        // EF Core interceptors
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.Services.AddScoped<IApplicationDbContext>(
            sp => sp.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        // JWT auth
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"]!;

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSection["Issuer"],
                    ValidAudience            = jwtSection["Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(
                                                  Encoding.UTF8.GetBytes(secret)),
                    ClockSkew                = TimeSpan.Zero,
                };
            });

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<IPasswordHasher, BcryptPasswordHasher>();
    }
}
