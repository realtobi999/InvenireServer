using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

public class Forbidden403Exception : Exception, IHttpException
{
    public Forbidden403Exception() : base("You do not have permission to access this resource.")
    {
    }

    public Forbidden403Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.Forbidden;
    public string Title => "Forbidden";
}