using System.Net;
using InvenireServer.Domain.Core.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Provides extension methods for configuring the application pipeline.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the application to throw a <see cref="NotAuthorized401Exception"/> when a response with status code 401 (Unauthorized) is generated.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <exception cref="NotAuthorized401Exception">Thrown when the response status code is 401.</exception>
    public static void ConfigureStatusCodePages(this WebApplication app)
    {
        app.UseStatusCodePages(context =>
        {
            var response = context.HttpContext.Response;

            switch (response.StatusCode)
            {
                case (int)HttpStatusCode.Unauthorized:
                    throw new NotAuthorized401Exception();
                case (int)HttpStatusCode.Forbidden:
                    throw new Forbidden403Exception();
            }

            return Task.CompletedTask;
        });
    }
}
