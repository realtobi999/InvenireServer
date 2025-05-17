namespace InvenireServer.Domain.Core.Dtos.Common;

/// <summary>
/// Represents an error message containing details about an error response.
/// </summary>
public record ErrorMessageDto
{
    /// <summary>
    /// Gets or sets the HTTP status code associated with the error.
    /// </summary>
    public required int Status { get; init; }

    /// <summary>
    /// Gets or sets the type of the error, which can provide additional context.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets or sets the title of the error, usually a short summary of the problem.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the detailed description of the error.
    /// </summary>
    public required string Detail { get; init; }

    /// <summary>
    /// Gets or sets additional errors, for example messages during a validation error.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// Gets or sets a URI reference that identifies the specific occurrence of the error.
    /// </summary>
    public required string Instance { get; init; }
}