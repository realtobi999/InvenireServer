using System.Net;
using InvenireServer.Domain.Exceptions.Http;

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
}
