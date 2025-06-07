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

            switch (response.StatusCode)
            {
                case (int)HttpStatusCode.Unauthorized:
                    throw new Unauthorized401Exception();
                case (int)HttpStatusCode.Forbidden:
                    throw new Forbidden403Exception();
            }

            return Task.CompletedTask;
        });
    }
}