using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using InvenireServer.Application.Dtos.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Presentation.Middleware;

/// <summary>
/// Handles exceptions and maps them to error responses.
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Handles an exception and writes an error response.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    /// <param name="exception">Exception to handle.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Awaitable task indicating whether the exception was handled.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        switch (exception)
        {
            case ValidationException:
                await HandleValidationException(context, (ValidationException)exception, token);
                break;

            case IHttpException:
                await HandleHttpException(context, (IHttpException)exception, token);
                break;

            case DbUpdateException or DbUpdateConcurrencyException:
                await HandleHttpException(context, new Conflict409Exception(), token);
                break;

            default:
                await HandleException(context, exception, token);
                break;
        }

        return await ValueTask.FromResult(false);
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException exception, CancellationToken token)
    {
        var error = new ErrorMessageDto
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = nameof(ValidationException),
            Title = "Bad request",
            Detail = exception.Message,
            Errors = [.. exception.Errors.Select(e => e.ToString())],
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
            Title = "An unexpected internal error occurred.",
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
