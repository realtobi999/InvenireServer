using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using InvenireServer.Application;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Common;
using InvenireServer.Infrastructure.Authentication.Options;
using InvenireServer.Infrastructure.Email;
using InvenireServer.Infrastructure.Persistence;
using InvenireServer.Presentation.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InvenireServer.Presentation.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<InvenireServerContext>(opt =>
        {
            opt.UseNpgsql(
                connectionString,
                builder => { builder.EnableRetryOnFailure(3); }
            );
        });
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

    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Admin>, AdminValidator>();
        services.AddScoped<IValidator<Employee>, EmployeeValidator>();
        services.AddScoped<IValidator<Organization>, OrganizationValidator>();
        services.AddScoped<IValidatorFactory, ValidatorFactory>();
    }

    public static void ConfigureHashing(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();
        services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
    }

    public static void ConfigureErrorHandling(this IServiceCollection services)
    {
        // Configure the validation performed by validation attributes.
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                if (errors.Any(HasDefaultErrorMessages))
                    errors =
                    [
                        .. errors.Where(err => !HasDefaultErrorMessages(err)),
                        "Request body is empty or missing fields."
                    ];

                throw new ValidationException(errors);

                // Filter out default error messages revealing internal info about JSON deserialization or missing DTO fields.
                static bool HasDefaultErrorMessages(string err)
                {
                    return err.Contains("JSON deserialization for type") || err.Contains("The dto field is required");
                }
            };
        });
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
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(ApplicationAssembly).Assembly);
        });
    }
}