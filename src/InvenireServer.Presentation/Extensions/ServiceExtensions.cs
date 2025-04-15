using InvenireServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Presentation.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers the database context with PostgreSQL configuration.
    /// </summary>
    public static void ConfigureDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<InvenireServerContext>(opt =>
        {
            opt.UseNpgsql(
                connectionString,
                builder =>
                {
                    builder.EnableRetryOnFailure(maxRetryCount: 3);
                }
            );
        });
    }
}
