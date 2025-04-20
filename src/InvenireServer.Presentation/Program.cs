using Serilog;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Presentation.Middleware;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Persistence.Managers;

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

                builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
                builder.Services.ConfigureJwt(builder.Configuration);
                builder.Services.AddControllers();
                builder.Services.AddExceptionHandler<ExceptionHandler>();
                builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);
            }
            var app = builder.Build();
            {
                app.UseSerilogRequestLogging();

                // Production environment configuration.
                if (app.Environment.IsProduction())
                {
                    app.UseExceptionHandler(_ => { });
                }

                app.ConfigureStatusCodePages();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Unhandled exception while starting the application,");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}