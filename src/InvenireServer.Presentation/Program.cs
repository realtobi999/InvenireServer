using Serilog;
using InvenireServer.Application.Services;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Persistence.Managers;
using InvenireServer.Application.Core.Authentication;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Presentation;

/// <summary>
/// The application entry point class responsible for configuring and running the web application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point method which configures services, middleware, and runs the web application.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    private static void Main(string[] args)
    {
        try
        {
            Log.Information("-----------------------------------------------------------");

            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Host.ConfigureSerilog(builder.Configuration);
                builder.Host.ConfigureConfiguration();

                builder.Services.ConfigureJwt(builder.Configuration);
                builder.Services.ConfigureMappers();
                builder.Services.ConfigureHashing();
                builder.Services.ConfigureValidators();
                builder.Services.ConfigureRareLimiters();
                builder.Services.ConfigureEmailService(builder.Configuration);
                builder.Services.ConfigureErrorHandling();
                builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);

                builder.Services.AddScoped<IJwtManager, JwtManager>();
                builder.Services.AddScoped<IServiceManager, ServiceManager>();
                builder.Services.AddScoped<IFactoryManager, FactoryManager>();
                builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
                builder.Services.AddControllers();
                builder.Services.AddHostedService<EmployeeCleanupService>();
            }
            var app = builder.Build();
            {
                app.UseSerilogRequestLogging();

                if (app.Environment.IsProduction())
                {
                    app.UseExceptionHandler(_ => { });
                }

                app.ConfigureStatusCodePages();
                app.UseAuthorization();
                app.UseRateLimiter();
                app.MapControllers();
                app.Run();
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Unhandled exception while starting the application.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
