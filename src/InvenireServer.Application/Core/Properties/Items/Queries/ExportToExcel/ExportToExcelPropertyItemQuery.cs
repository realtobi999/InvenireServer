using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToExcel;

public record ExportToExcelPropertyItemQuery : IRequest<Stream>
{
    public required Jwt Jwt { get; init; }
}
