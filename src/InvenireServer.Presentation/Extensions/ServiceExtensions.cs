using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using InvenireServer.Infrastructure.Email;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Presentation.Middleware;
using InvenireServer.Application.Core.Mappers;
using InvenireServer.Infrastructure.Persistence;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Application.Core.Validators;
using InvenireServer.Domain.Core.Interfaces.Email;
using InvenireServer.Domain.Core.Exceptions.Common;
using InvenireServer.Domain.Core.Interfaces.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Provides extension methods to register and configure core services in the application.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the Entity Framework database context using the specified PostgreSQL connection string.
    /// </summary>
    /// <param name="services">The service collection to add the database context to.</param>
    /// <param name="connectionString">The connection string for the PostgreSQL database.</param>
    public static void ConfigureDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<InvenireServerContext>(opt =>
        {
            opt.UseNpgsql(
                connectionString,
                builder => { builder.EnableRetryOnFailure(maxRetryCount: 3); }
            );
        });
    }

    /// <summary>
    /// Configures JWT authentication and authorization services based on the provided application configuration.
    /// </summary>
    /// <param name="services">The service collection to add authentication and authorization to.</param>
    /// <param name="configuration">The application configuration used to initialize JWT settings.</param>
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var factory = new JwtFactory(configuration);
        services.AddSingleton<IJwtFactory>(factory);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = factory.Issuer,
                    ValidAudience = factory.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(factory.SigningKey))
                };
            });
        services.AddAuthorizationBuilder()
            .AddPolicy(JwtFactory.Policies.EMPLOYEE, policy => policy.RequireRole(JwtFactory.Policies.EMPLOYEE));
    }

    /// <summary>
    /// Registers mapping services, including the mapper factory and entity-to-DTO mappers.
    /// </summary>
    /// <param name="services">The service collection to register mapping services into.</param>
    public static void ConfigureMappers(this IServiceCollection services)
    {
        services.AddScoped<IMapperFactory, MapperFactory>();
        services.AddScoped<IMapper<Employee, RegisterEmployeeDto>, EmployeeMapper>();
    }

    /// <summary>
    /// Registers validator services, including the validator factory and entity validators.
    /// </summary>
    /// <param name="services">The service collection to register validation services into.</param>
    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidatorFactory, ValidatorFactory>();
        services.AddScoped<IValidator<Employee>, EmployeeValidator>();
    }

    /// <summary>
    /// Registers password hashing services used for hashing and verifying employee passwords.
    /// </summary>
    /// <param name="services">The service collection to register hashing services into.</param>
    public static void ConfigureHashing(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
    }

    /// <summary>
    /// Configures centralized error handling and validation response behavior for the application.
    /// Throws a <see cref="ValidationException"/> on invalid model states with customized error messages.
    /// </summary>
    /// <param name="services">The service collection to configure error handling on.</param>
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
                    {
                        errors =
                        [
                            .. errors.Where(err => !HasDefaultErrorMessages(err)),
                            "Request body is empty or missing fields."
                        ];
                    }

                    throw new ValidationException(errors);

                    // Filter out default error messages revealing internal info about JSON deserialization or missing DTO fields.
                    bool HasDefaultErrorMessages(string err) => err.Contains("JSON deserialization for type") || err.Contains("The dto field is required");
                };
        });
        services.AddExceptionHandler<ExceptionHandler>();
    }

    /// <summary>
    /// Configures rate limiting policies for the application, including limiting login attempts per IP address.
    /// </summary>
    /// <param name="services">The service collection to add rate limiting policies to.</param>
    public static void ConfigureRareLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Policy limiting login attempts per IP address: allow 5 immediate attempts,
            // then replenish 1 token every 15 minutes, effectively limiting to 1 attempt per 15 minutes after the first 5.
            options.AddPolicy("LoginPolicy", context =>
            {
                var address = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(address, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    QueueLimit = 0,
                    TokensPerPeriod = 1,
                    AutoReplenishment = true,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(15)
                });
            });
        });
    }

    /// <summary>
    /// Registers email sending and management services configured via application settings.
    /// </summary>
    /// <param name="services">The service collection to register email-related services into.</param>
    /// <param name="configuration">The application configuration used to initialize SMTP settings.</param>
    public static void ConfigureEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailSender>(_ => EmailSenderFactory.Initiate(configuration));
        services.AddScoped<IEmailManager, EmailManager>();
    }
}
