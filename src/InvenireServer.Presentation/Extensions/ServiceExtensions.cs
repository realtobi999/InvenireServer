using System.Text;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Application.Core.Mappers;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Exceptions.Common;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Factories;
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

                    // Filter out default error message for empty  request  body which reveals internal information.
                    var predicate = (string err) => err.Contains("JSON deserialization for type") || err.Contains("The dto field is required");
                    if (errors.Any(predicate))
                    {
                        errors = [.. errors.Where(err => !predicate(err))];
                        errors.Add("Request body is empty or missing fields.");
                    }

                    throw new ValidationException(errors);
                };
        });
        services.AddExceptionHandler<ExceptionHandler>();
    }
}