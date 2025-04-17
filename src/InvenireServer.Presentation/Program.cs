using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Infrastructure.Persistence.Managers;
using InvenireServer.Presentation.Extensions;

namespace InvenireServer.Presentation;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Host.ConfigureConfiguration();

            builder.Services.AddControllers();
            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
            builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);
        }
        var app = builder.Build();
        {
            app.MapControllers();
            app.Run();
        }
    }
}