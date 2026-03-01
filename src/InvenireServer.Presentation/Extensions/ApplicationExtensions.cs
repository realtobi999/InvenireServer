using System.Net;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Defines application pipeline extension methods.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Configures status code pages to throw HTTP exceptions.
    /// </summary>
    /// <param name="app">Application to configure.</param>
    public static void ConfigureStatusCodePages(this WebApplication app)
    {
        app.UseStatusCodePages(context =>
        {
            var response = context.HttpContext.Response;

            return response.StatusCode switch
            {
                (int)HttpStatusCode.Forbidden => throw new Forbidden403Exception(),
                (int)HttpStatusCode.Unauthorized => throw new Unauthorized401Exception(),
                (int)HttpStatusCode.MethodNotAllowed => throw new MethodNotAllowed405Exception(),
                _ => Task.CompletedTask,
            };
        });
    }

    /// <summary>
    /// Applies any pending Entity Framework Core migrations at startup.
    /// </summary>
    /// <param name="app">Application to migrate.</param>
    public static void RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InvenireServerContext>();
        db.Database.Migrate();
    }
}
