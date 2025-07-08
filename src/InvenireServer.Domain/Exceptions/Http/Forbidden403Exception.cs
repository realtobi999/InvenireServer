using InvenireServer.Domain.Interfaces.Exceptions;
using Microsoft.AspNetCore.Http;

namespace InvenireServer.Domain.Exceptions.Http;

public class Forbidden403Exception : Exception, IHttpException
{
    public Forbidden403Exception() : base("You do not have permission to access this resource.")
    {
    }

    public Forbidden403Exception(string message) : base(message)
    {
    }

    public int StatusCode => StatusCodes.Status403Forbidden;

    public string Title => "Forbidden";
}