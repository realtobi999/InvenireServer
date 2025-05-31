using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Http;

/// <summary>
/// Exception thrown when a requested resource cannot be found.
/// Corresponds to HTTP status 404 Not Found.
/// </summary>
public class NotFound404Exception : Exception, IHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFound404Exception"/> class with no message.
    /// </summary>
    public NotFound404Exception()
    {
    }

    /// <inheritdoc/>
    public NotFound404Exception(string entity) : base($"The requested {entity} was not found in the system.")
    {
    }

    /// <inheritdoc/>
    public string Title => "Resource Not Found";

    /// <inheritdoc/>
    public int StatusCode => (int)HttpStatusCode.NotFound;
}