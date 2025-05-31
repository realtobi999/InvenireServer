using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Http;

public class NotFound404Exception : Exception, IHttpException
{
    public NotFound404Exception()
    {
    }

    public NotFound404Exception(string entity) : base($"The requested {entity} was not found in the system.")
    {
    }

    public string Title => "Resource Not Found";
    public int StatusCode => (int)HttpStatusCode.NotFound;
}