namespace InvenireServer.Domain.Interfaces.Exceptions;

/// <summary>
/// Defines a contract for HTTP exceptions.
/// </summary>
public interface IHttpException
{
    int StatusCode { get; }

    string Title { get; }

    string Message { get; }
}
