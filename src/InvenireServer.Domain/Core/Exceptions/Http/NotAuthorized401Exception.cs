using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Http;

/// <summary>
/// Exception thrown when a user attempts an action without the required authorization.
/// </summary>
public class NotAuthorized401Exception : Exception, IHttpException
{
    /// <summary>
    /// Initializes a new instance of <see cref="NotAuthorized401Exception"/> with a default message indicating lack of authorization.
    /// </summary>
    public NotAuthorized401Exception() : base("You are not authorized to perform the action. Please ensure you have the necessary permissions.")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NotAuthorized401Exception"/> with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NotAuthorized401Exception(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public string Title => "Unauthorized Access";

    /// <inheritdoc/>
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
}