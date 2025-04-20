using System.Net;
using InvenireServer.Domain.Core.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures custom behavior for specific HTTP status codes.
    /// </summary>
    public static void ConfigureStatusCodePages(this WebApplication app)
    {
        app.UseStatusCodePages(context =>
        {
            var response = context.HttpContext.Response;

            if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                throw new NotAuthorized401Exception();
            }

            return Task.CompletedTask;
        });
    }
}