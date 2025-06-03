using InvenireServer.Domain.Interfaces;
using InvenireServer.Domain.Interfaces.Exceptions;
using Microsoft.AspNetCore.Http;

namespace InvenireServer.Domain.Exceptions.Http;

/// <summary>
/// Represents an exception corresponding to an HTTP 403 Forbidden error.
/// </summary>
public class Forbidden403Exception : Exception, IHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Forbidden403Exception"/> class with a default message.
    /// </summary>
    public Forbidden403Exception() : base("You do not have permission to access this resource.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Forbidden403Exception"/> class with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public Forbidden403Exception(string? message) : base(message)
    {
    }
    /// <inheritdoc/>
    public int StatusCode => (int)StatusCodes.Status403Forbidden;

    /// <inheritdoc/>
    public string Title => "Forbidden";
}
