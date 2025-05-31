using System.Net;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Exceptions.Common;

/// <summary>
/// Represents an exception that is thrown when one or more validation errors occurs.
/// </summary>
public class ValidationException : Exception, IValidationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a list of validation error messages.
    /// </summary>
    /// <param name="errors">The list of validation error messages.</param>
    public ValidationException(List<string> errors) : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    /// <inheritdoc/>
    public int StatusCode => (int)HttpStatusCode.BadRequest;

    /// <inheritdoc/>
    public string Title => "Bad Request";

    /// <inheritdoc/>
    public List<string> Errors { get; }
}
