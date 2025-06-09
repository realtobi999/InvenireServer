using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

public class NotFound404Exception : Exception, IHttpException
{
    public NotFound404Exception() : base("The requested resource was not found in the system.")
    {
    }

    public NotFound404Exception(string entity) : base($"The requested {entity} was not found in the system.")
    {
    }

    public string Title => "Resource Not Found";

    public int StatusCode => (int)HttpStatusCode.NotFound;
}