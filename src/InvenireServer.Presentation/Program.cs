using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services;
using InvenireServer.Application.Services.Admins.Backgrounds;
using InvenireServer.Application.Services.Employees.Backgrounds;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Infrastructure.Persistence.Repositories;
using InvenireServer.Presentation.Extensions;
using Serilog;

namespace InvenireServer.Presentation;

public class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Host.ConfigureSerilog(builder.Configuration);
                builder.Host.ConfigureConfiguration();

                builder.Services.ConfigureJwt(builder.Configuration);
                builder.Services.ConfigureHashing();
                builder.Services.ConfigureMediatR();
                builder.Services.ConfigureValidators();
                builder.Services.ConfigureRareLimiters();
                builder.Services.ConfigureEmailService(builder.Configuration);
                builder.Services.ConfigureErrorHandling();
                builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);

                builder.Services.AddScoped<IJwtManager, JwtManager>();
                builder.Services.AddScoped<IServiceManager, ServiceManager>();
                builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
                builder.Services.AddControllers(options =>
                {
                    // options.Filters.Add<RequestBodyValidationFilter>();
                });
                builder.Services.AddHostedService<AdminCleanupBackgroundService>();
                builder.Services.AddHostedService<EmployeeCleanupBackgroundService>();
            }
            var app = builder.Build();
            {
                app.UseSerilogRequestLogging();

                app.UseExceptionHandler(_ => { });

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