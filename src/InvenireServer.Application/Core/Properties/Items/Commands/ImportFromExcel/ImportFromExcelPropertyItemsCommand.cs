using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromExcel;

public record ImportFromExcelPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
    public required Stream Stream { get; set; }
    public required string ColumnString { get; set; }
}
