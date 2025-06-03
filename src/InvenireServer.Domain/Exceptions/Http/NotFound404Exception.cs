using System.Net;
using InvenireServer.Domain.Interfaces;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

/// <summary>
/// Represents an exception for an HTTP 404 Not Found error.
/// </summary>
public class NotFound404Exception : Exception, IHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFound404Exception"/> class with a default message.
    /// </summary>
    public NotFound404Exception() : base("The requested resource was not found in the system.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFound404Exception"/> class with a message including the entity name.
    /// </summary>
    /// <param name="entity">The name of the entity that was not found.</param>
    public NotFound404Exception(string entity) : base($"The requested {entity} was not found in the system.")
    {
    }

    /// <inheritdoc/>
    public string Title => "Resource Not Found";

    /// <inheritdoc/>
    public int StatusCode => (int)HttpStatusCode.NotFound;
}