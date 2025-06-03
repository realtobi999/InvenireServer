using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Common;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Interfaces;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Presentation.Middleware;

/// <summary>
/// Handles exceptions and returns a standardized JSON error response to the client.
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        switch (exception)
        {
            case IValidationException:
                await HandleValidationException(context, (IValidationException)exception, token);
                break;

            case IHttpException:
                await HandleHttpException(context, (IHttpException)exception, token);
                break;

            default:
                await HandleException(context, exception, token);
                break;
        }

        return await ValueTask.FromResult(false);
    }

    private static async Task HandleValidationException(HttpContext context, IValidationException exception, CancellationToken token)
    {
        var error = new ErrorMessageDto
        {
            Status = exception.StatusCode,
            Type = exception.GetType().Name,
            Title = exception.Title,
            Detail = exception.Message,
            Errors = exception.Errors,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        await WriteErrorAsync(context, error, token);
    }

    private static async Task HandleHttpException(HttpContext context, IHttpException exception, CancellationToken token)
    {
        var error = new ErrorMessageDto
        {
            Status = exception.StatusCode,
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
            Status = (int)HttpStatusCode.InternalServerError,
            Type = exception.GetType().Name,
            Title = "An unexpected internal error occurred",
            Detail = exception.Message,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        await WriteErrorAsync(context, error, token);
    }

    private static async Task WriteErrorAsync(HttpContext context, ErrorMessageDto error, CancellationToken token)
    {
        context.Response.StatusCode = error.Status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(error, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        }, token);
    }
}