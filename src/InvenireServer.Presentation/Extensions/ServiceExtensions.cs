using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using InvenireServer.Application;
using InvenireServer.Application.Behaviors;
using InvenireServer.Application.Interfaces.Common.Transactions;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Constants;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication.Options;
using InvenireServer.Infrastructure.Email;
using InvenireServer.Infrastructure.Persistence;
using InvenireServer.Infrastructure.Persistence.Transactions;
using InvenireServer.Presentation.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace InvenireServer.Presentation.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsConstants.Policies.FRONTEND_POLICY, policy =>
            {
                policy.WithOrigins(configuration.GetSection("Frontend:BaseAddress").Value ?? throw new NullReferenceException())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public static void ConfigureDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<InvenireServerContext>(opt =>
        {
            opt.UseNpgsql(
                connectionString,
                builder => { builder.EnableRetryOnFailure(3); }
            );
        });
        services.AddScoped<ITransactionScope, InvenireTransactionScope>();
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new NullReferenceException("JWT Configuration is missing or incomplete.");
        services.AddSingleton(options);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey))
                };
                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue(CookieConstants.JWT, out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorizationBuilder()
            .AddPolicy(Jwt.Policies.EMPLOYEE, policy =>
            {
                policy.RequireRole(Jwt.Roles.EMPLOYEE);
                policy.RequireClaim("is_verified", bool.TrueString);
            })
            .AddPolicy(Jwt.Policies.UNVERIFIED_EMPLOYEE, policy =>
            {
                policy.RequireRole(Jwt.Roles.EMPLOYEE);
                policy.RequireClaim("is_verified", bool.FalseString);
            })
            .AddPolicy(Jwt.Policies.ADMIN, policy =>
            {
                policy.RequireRole(Jwt.Roles.ADMIN);
                policy.RequireClaim("is_verified", bool.TrueString);
            })
            .AddPolicy(Jwt.Policies.UNVERIFIED_ADMIN, policy =>
            {
                policy.RequireRole(Jwt.Roles.ADMIN);
                policy.RequireClaim("is_verified", bool.FalseString);
            });
    }

    public static void ConfigureHashing(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();
        services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
    }

    public static void ConfigureErrorHandling(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        services.AddExceptionHandler<ExceptionHandler>();
    }

    public static void ConfigureRareLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;

            // Policy limiting login attempts per IP address: allow 5 immediate attempts, then replenishes 5 tokens every 15 minutes.
            options.AddPolicy("LoginPolicy", context =>
            {
                var address = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(address, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    QueueLimit = 0,
                    TokensPerPeriod = 5,
                    AutoReplenishment = true,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(15)
                });
            });
            // Policy limiting sending a verification email per IP address: allows 3 immediate attempts, then replenishes 1 token every 12 hours.
            options.AddPolicy("SendVerificationPolicy", context =>
            {
                var address = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(address, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 3,
                    QueueLimit = 0,
                    TokensPerPeriod = 1,
                    AutoReplenishment = true,
                    ReplenishmentPeriod = TimeSpan.FromHours(12)
                });
            });
        });
    }

    public static void ConfigureEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailSender>(_ => EmailSenderFactory.Initiate(configuration));
        services.AddScoped<IEmailManager, EmailManager>();
    }

    public static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => { options.RegisterServicesFromAssembly(typeof(ApplicationAssembly).Assembly); });
        services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);
        services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>)));
        services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)));
    }

    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }
}