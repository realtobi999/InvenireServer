using ClosedXML.Excel;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromExcel;

/// <summary>
/// Handler for the request to import property items from Excel.
/// </summary>
public class ImportFromExcelPropertyItemsCommandHandler : IRequestHandler<ImportFromExcelPropertyItemsCommand>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryManager _repositories;

    public ImportFromExcelPropertyItemsCommandHandler(IMediator mediator, IRepositoryManager repositories)
    {
        _mediator = mediator;
        _repositories = repositories;
    }

    public static readonly string[] PropertyKeys =
    [
        "InventoryNumber",
        "RegistrationNumber",
        "Name",
        "Price",
        "SerialNumber",
        "DateOfPurchase",
        "DateOfSale",
        "LocationRoom",
        "LocationBuilding",
        "LocationAdditionalNote",
        "Description",
        "DocumentNumber",
        "EmployeeEmailAddress"
    ];

    public static readonly string[] RequiredPropertyKey =
    [
        "InventoryNumber",
        "RegistrationNumber",
        "Name",
        "Price",
        "DateOfPurchase",
        "LocationRoom",
        "LocationBuilding",
    ];

    /// <summary>
    /// Handles the request to import property items from Excel.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(ImportFromExcelPropertyItemsCommand request, CancellationToken ct)
    {
        // Extract headers from the column string and validate:
        //  1. All required property keys are present.
        //  2. The total number of columns does not exceed the maximum allowed.
        var headers = new Dictionary<string, string>();
        foreach (var column in request.ColumnString.Split("|"))
        {
            var parts = column.Split(";");
            headers[parts[0]] = parts[1];
        }

        var missing = RequiredPropertyKey.Where(k => !headers.ContainsKey(k) || string.IsNullOrWhiteSpace(headers[k]));
        if (missing.Any())
            throw new BadRequest400Exception($"The excel file is missing required columns: {string.Join(", ", missing)}");
        if (headers.Count > PropertyKeys.Length)
            throw new BadRequest400Exception($"The excel file contains too many columns. Expected up to {PropertyKeys.Length}, but found {headers.Count}.");

        // Extract row values using the  headers  and  build  the  commands  for
        // creating the items. Set any missing required fields to an empty value
        // of that type so they are caught by the command validator later.
        var commands = new List<CreatePropertyItemCommand>();

        using var workbook = new XLWorkbook(request.Stream);
        foreach (var row in workbook.Worksheets.First().RowsUsed().Skip(1))
        {
            // Small functions for retrieving the data from the excel file.
            string? GetValue(string key)
            {
                if (!headers.TryGetValue(key, out var columnLetter))
                    return null;

                if (string.IsNullOrWhiteSpace(columnLetter))
                    return null;

                var value = row.Cell(columnLetter).GetString();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }

            double GetDouble(string key)
            {
                return double.TryParse(GetValue(key), out var value) ? value : 0;
            }

            DateTimeOffset? GetDate(string key)
            {
                return DateTimeOffset.TryParse(GetValue(key), out var value) ? value : null;
            }

            var email = GetValue("EmployeeEmailAddress");
            var employee = (Employee?)null;
            if (email is not null)
                employee = await _repositories.Employees.GetAsync(e => e.EmailAddress == email) ?? throw new NotFound404Exception($"The employee was not found in the system. (email - {email})");

            commands.Add(new CreatePropertyItemCommand
            {
                InventoryNumber = GetValue("InventoryNumber") ?? string.Empty,
                RegistrationNumber = GetValue("RegistrationNumber") ?? string.Empty,
                Name = GetValue("Name") ?? string.Empty,
                Price = GetDouble("Price"),
                SerialNumber = GetValue("SerialNumber"),
                DateOfPurchase = GetDate("DateOfPurchase") ?? DateTimeOffset.MinValue,
                DateOfSale = GetDate("DateOfSale"),
                Location = new CreatePropertyItemCommandLocation
                {
                    Room = GetValue("LocationRoom") ?? string.Empty,
                    Building = GetValue("LocationBuilding") ?? string.Empty,
                    AdditionalNote = GetValue("LocationAdditionalNote")
                },
                Description = GetValue("Description"),
                DocumentNumber = GetValue("DocumentNumber"),
                EmployeeId = employee?.Id
            });
        }

        await _mediator.Send(new CreatePropertyItemsCommand
        {
            Jwt = request.Jwt,
            Items = commands,
        }, ct);
    }
}

