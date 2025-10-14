using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

public class NotFound404Exception : Exception, IHttpException
{
    public NotFound404Exception() : base("The requested resource was not found in the system.")
    {
    }

    public NotFound404Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.NotFound;
    public string Title => "Resource Not Found";
}