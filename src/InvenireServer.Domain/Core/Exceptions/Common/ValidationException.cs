using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Common;

public class ValidationException : Exception, IValidationException
{
    public ValidationException(List<string> errors) : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public int StatusCode => (int)HttpStatusCode.BadRequest;
    public string Title => "Bad Request";
    public List<string> Errors { get; }
}
