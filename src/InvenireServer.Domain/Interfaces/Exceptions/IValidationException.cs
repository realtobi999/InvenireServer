namespace InvenireServer.Domain.Interfaces.Exceptions;

public interface IHttpValidationException : IHttpException
{
    List<string> Errors { get; }
}