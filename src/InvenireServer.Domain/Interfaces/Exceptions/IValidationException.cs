namespace InvenireServer.Domain.Interfaces.Exceptions;

public interface IValidationException : IHttpException
{
    List<string> Errors { get; }
}