using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

/// <summary>
/// Represents an HTTP 405 Method Not Allowed error.
/// </summary>
public class MethodNotAllowed405Exception : Exception, IHttpException
{
    public MethodNotAllowed405Exception() : base("The method for this endpoint is not allowed.")
    {
    }

    public MethodNotAllowed405Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.MethodNotAllowed;
    public string Title => "Method not allowed";
}
