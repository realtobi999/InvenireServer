using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

/// <summary>
/// Represents an HTTP 401 Unauthorized error.
/// </summary>
public class Unauthorized401Exception : Exception, IHttpException
{
    public Unauthorized401Exception() : base("You are not authorized to perform the action. Please ensure you have the necessary permissions.")
    {
    }

    public Unauthorized401Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.Unauthorized;
    public string Title => "Unauthorized Access";
}
