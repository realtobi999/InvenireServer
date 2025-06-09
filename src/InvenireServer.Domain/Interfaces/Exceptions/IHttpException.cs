namespace InvenireServer.Domain.Interfaces.Exceptions;

public interface IHttpException
{
    int StatusCode { get; }

    string Title { get; }

    string Message { get; }
}