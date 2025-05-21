namespace InvenireServer.Domain.Core.Interfaces.Common;

public interface IValidationException : IHttpException
{
    /// <summary>
    /// Stores the validation error messages.
    /// </summary>
    List<string> Errors { get; }
}
