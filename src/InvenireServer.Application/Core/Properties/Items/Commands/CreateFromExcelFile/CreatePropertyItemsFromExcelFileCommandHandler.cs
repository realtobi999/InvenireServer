using ClosedXML.Excel;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;

namespace InvenireServer.Application.Core.Properties.Items.Commands.CreateFromExcelFile;

public class CreatePropertyItemsFromExcelFileCommandHandler : IRequestHandler<CreatePropertyItemsFromExcelFileCommand>
{
    private readonly IMediator _mediator;

    public CreatePropertyItemsFromExcelFileCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CreatePropertyItemsFromExcelFileCommand request, CancellationToken ct)
    {
        var headers = new Dictionary<string, string>();

        foreach (var column in request.ColumnString.Split("|"))
        {
            var parts = column.Split(";");
            headers[parts[0]] = parts[1];
        }

        using var workbook = new XLWorkbook(request.Stream);
        var commands = workbook.Worksheets.First().RowsUsed().Skip(1).Select(row =>
        {
            string? GetValue(string key)
            {
                if (!headers.TryGetValue(key, out var columnLetter) || string.IsNullOrWhiteSpace(columnLetter))
                    return null;

                var value = row.Cell(columnLetter).GetString();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }

            double GetDouble(string key) => double.TryParse(GetValue(key), out var result) ? result : 0;
            DateTimeOffset GetDate(string key) => DateTimeOffset.TryParse(GetValue(key), out var result) ? result : DateTimeOffset.MinValue;
            DateTimeOffset? GetNullableDate(string key) => DateTimeOffset.TryParse(GetValue(key), out var result) ? result : null;

            return new CreatePropertyItemCommand
            {
                InventoryNumber = GetValue("InventoryNumber") ?? string.Empty,
                RegistrationNumber = GetValue("RegistrationNumber") ?? string.Empty,
                Name = GetValue("Name") ?? string.Empty,
                Price = GetDouble("Price"),
                SerialNumber = GetValue("SerialNumber"),
                DateOfPurchase = GetDate("DateOfPurchase"),
                DateOfSale = GetNullableDate("DateOfSale"),
                Location = new CreatePropertyItemCommandLocation
                {
                    Room = GetValue("LocationRoom") ?? string.Empty,
                    Building = GetValue("LocationBuilding") ?? string.Empty,
                    AdditionalNote = GetValue("LocationAdditionalNote")
                },
                Description = GetValue("Description"),
                DocumentNumber = GetValue("DocumentNumber") ?? string.Empty,
            };
        }).ToList();


        await _mediator.Send(new CreatePropertyItemsCommand
        {
            Jwt = request.Jwt,
            Items = commands,
        }, ct);
    }
}
