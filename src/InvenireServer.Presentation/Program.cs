using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Persistence.Managers;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Presentation.Middleware;

namespace InvenireServer.Presentation;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Host.ConfigureConfiguration();

            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
            builder.Services.AddControllers();
            builder.Services.AddExceptionHandler<ExceptionHandler>();
            builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);
        }
        var app = builder.Build();
        {
            // Production environment configuration.
            if (app.Environment.IsProduction())
            {
                app.UseExceptionHandler(_ => { });
            }

            app.MapControllers();
            app.Run();
        }
    }
}