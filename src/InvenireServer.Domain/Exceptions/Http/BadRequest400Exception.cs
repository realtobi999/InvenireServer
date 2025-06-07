using System.Net;
using InvenireServer.Domain.Interfaces.Exceptions;

namespace InvenireServer.Domain.Exceptions.Http;

public class BadRequest400Exception : Exception, IHttpException
{
    public BadRequest400Exception() : base("The request could not be understood or was missing required parameters.")
    {
    }

    public BadRequest400Exception(string message) : base(message)
    {
    }

    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public string Title => "Bad Request";
}