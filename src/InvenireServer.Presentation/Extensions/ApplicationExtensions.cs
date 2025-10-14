using System.Net;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Presentation.Extensions;

public static class ApplicationExtensions
{
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