using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromExcel;

/// <summary>
/// Represents a request to import property items from Excel.
/// </summary>
public record ImportFromExcelPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
    public required Stream Stream { get; set; }
    public required string ColumnString { get; set; }
}
