namespace InvenireServer.Domain.Interfaces.Exceptions;

/// <summary>
/// Represents an HTTP exception containing one or more validation error messages.
/// </summary>
public interface IValidationException : IHttpException
{
    /// <summary>
    /// Stores the validation error messages.
    /// </summary>
    List<string> Errors { get; }
}