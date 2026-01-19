using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToExcel;

/// <summary>
/// Represents a query to export property items to Excel.
/// </summary>
public record ExportToExcelPropertyItemQuery : IRequest<Stream>
{
    public required Jwt Jwt { get; init; }
}
