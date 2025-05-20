using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Http;

public class NotAuthorized401Exception : Exception, IHttpException
{
    public NotAuthorized401Exception() : base("You are not authorized to perform the action. Please ensure you have the necessary permissions.")
    {
    }

    public NotAuthorized401Exception(string message) : base(message)
    {
    }

    public string Title => "Unauthorized Access";
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
}