using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

/// <summary>
/// Represents an HTTP 409 Conflict error.
/// </summary>
public class Conflict409Exception : Exception, IHttpException
{
    public Conflict409Exception() : base("A conflict occurred while processing the request.")
    {
    }

    public Conflict409Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.Conflict;
    public string Title => "Conflict";
}
