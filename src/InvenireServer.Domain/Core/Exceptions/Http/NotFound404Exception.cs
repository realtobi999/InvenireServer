using System.Net;

namespace InvenireServer.Domain.Core.Exceptions.Http;

public class NotFound404Exception : Exception, IHttpException
{
    public NotFound404Exception()
    {
    }

    public NotFound404Exception(string message) : base(message)
    {
    }

    public NotFound404Exception(string entity, object key) : base($"The requested {entity} with the key '{key}' was not found in the system.")
    {
    }

    public string Title => "Resource Not Found";
    public int StatusCode => (int)HttpStatusCode.NotFound;
}