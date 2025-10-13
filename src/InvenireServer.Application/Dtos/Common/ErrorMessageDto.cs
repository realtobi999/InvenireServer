namespace InvenireServer.Application.Dtos.Common;

public record ErrorMessageDto
{
    public required int Status { get; init; }
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Detail { get; init; }
    public List<string>? Errors { get; init; }
    public required string Instance { get; init; }
}