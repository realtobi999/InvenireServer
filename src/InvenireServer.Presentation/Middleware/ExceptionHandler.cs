using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using InvenireServer.Domain.Core.Dtos.Common;
using InvenireServer.Domain.Core.Exceptions.Http;

namespace InvenireServer.Presentation.Middleware;

/// <summary>
/// Handles and logs exceptions, then returns a standardized JSON error response to the client.
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        if (exception is IHttpException httpException)
        {
            await HandleHttpException(context, httpException, token);
        }
        else
        {
            await HandleException(context, exception, token);
        }

        _logger.LogError("An error occurred at {Timestamp} while processing the request: {Method} {Path} - {Message}",
            DateTime.UtcNow,
            context.Request.Method,
            context.Request.Path,
            exception.Message);

        return await ValueTask.FromResult(false);
    }

    private static async Task HandleHttpException(HttpContext context, IHttpException exception, CancellationToken token)
    {
        var error = new ErrorMessageDto
        {
            StatusCode = exception.StatusCode,
            Type = exception.GetType().Name,
            Title = exception.Title,
            Detail = exception.Message,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        await WriteErrorAsync(context, error, token);
    }

    private static async Task HandleException(HttpContext context, Exception exception, CancellationToken token)
    {
        var error = new ErrorMessageDto
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Type = exception.GetType().Name,
            Title = "An unexpected internal error occurred",
            Detail = exception.Message,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        await WriteErrorAsync(context, error, token);
    }

    private static async Task WriteErrorAsync(HttpContext context, ErrorMessageDto error, CancellationToken token)
    {
        context.Response.StatusCode = error.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(error, token);
    }
}
