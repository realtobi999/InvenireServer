using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Http;

/// <summary>
/// Represents an exception corresponding to an HTTP 400 Bad Request error.
/// </summary>
public class BadRequest400Exception : Exception, IHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequest400Exception"/> class with no message.
    /// </summary>
    public BadRequest400Exception()
    {
    }

    /// <inheritdoc/>
    public BadRequest400Exception(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public int StatusCode => (int)HttpStatusCode.BadRequest;

    /// <inheritdoc/>
    public string Title => "Bad Request";
}
