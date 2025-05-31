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

public static class ServiceExtensions
{
    /// <summary>
    /// Registers the database context.
    /// </summary>
    /// <param name="connectionString">The connection string for the PostgresSQL database.</param>
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
    /// Configures JWT-based authentication and authorization using the provided configuration.
    /// </summary>
    /// <param name="configuration">The application's configuration used to initialize the JWT settings.</param>
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
    /// Registers mapping services, including the mapper factory and entity-to-dto mappers.
    /// </summary>
    public static void ConfigureMappers(this IServiceCollection services)
    {
        services.AddScoped<IMapperFactory, MapperFactory>();
        services.AddScoped<IMapper<Employee, RegisterEmployeeDto>, EmployeeMapper>();
    }

    /// <summary>
    /// Registers validator services, including the validator factory and entity validators.
    /// </summary>
    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidatorFactory, ValidatorFactory>();
        services.AddScoped<IValidator<Employee>, EmployeeValidator>();
    }

    /// <summary>
    /// Configures and registers hashing related classes.
    /// </summary>
    public static void ConfigureHashing(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
    }

    /// <summary>
    /// Configures centralized error handling for the application.
    /// </summary>
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

                    // Filter out default error message for empty request body which reveals internal information.
                    bool HasDefaultErrorMessages(string err) => err.Contains("JSON deserialization for type") || err.Contains("The dto field is required");
                };
        });
        services.AddExceptionHandler<ExceptionHandler>();
    }

    /// <summary>
    /// Configures rare limiters for the application. 
    /// </summary>
    public static void ConfigureRareLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Policy for login attempts per IP: allow 5 immediate attempts, then 1 new attempt every 15 minutes.
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
    /// Configures email service for the application.
    /// </summary>
    /// <param name="configuration">The application's configuration used to configure the SMTP settings.</param>
    public static void ConfigureEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailSender>(_ => EmailSenderFactory.Initiate(configuration));
        services.AddScoped<IEmailManager, EmailManager>();
    }
}