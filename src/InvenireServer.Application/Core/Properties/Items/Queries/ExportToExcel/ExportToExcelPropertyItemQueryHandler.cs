using System.Linq.Expressions;
using ClosedXML.Excel;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToExcel;

public class ExportToExcelPropertyItemQueryHandler : IRequestHandler<ExportToExcelPropertyItemQuery, Stream>
{
    private readonly IRepositoryManager _repositories;

    public ExportToExcelPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<Stream> Handle(ExportToExcelPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        // Create the excel worksheet and initiate the headers.
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"Invenire-Export-{DateTime.Now:yyyyMMdd}");

        SetHeaders(worksheet);
        ConfigureHeaders(worksheet);

        // Loop through all property items by  using  pagination.  Assign  their
        // employees and cache them to avoid  duplicate  database  calls.  Write
        // each item to the file while keeping track of the current row.
        var row = 2;
        var limit = QueryPaginationOptions.MAX_LIMIT;
        var employees = new List<EmployeeDto>();

        var total = await _repositories.Properties.Items.CountAsync(i => i.PropertyId == property.Id);
        for (int offset = 0; offset < total; offset += limit)
        {
            var batch = await _repositories.Properties.Items.IndexAsync(new QueryOptions<PropertyItem, PropertyItemDto>
            {
                Selector = PropertyItemDtoSelector,
                Ordering = new QueryOrderingOptions<PropertyItem>(i => i.Id),
                Filtering = new QueryFilteringOptions<PropertyItem>
                {
                    Filters =
                    [
                        i => i.PropertyId == property.Id,
                    ]
                },
                Pagination = new QueryPaginationOptions(limit, offset),
            });
            if (!batch.Any()) break;

            foreach (var item in batch)
            {
                if (item.EmployeeId is not null)
                {
                    var employee = employees.FirstOrDefault(e => e.Id == item.EmployeeId);

                    if (employee is not null)
                    {
                        item.Employee = employee;
                    }
                    else
                    {
                        employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
                        {
                            Selector = EmployeeDtoSelector,
                            Filtering = new QueryFilteringOptions<Employee>
                            {
                                Filters = [e => e.Id == item.EmployeeId]
                            },
                        }) ?? throw new NotFound404Exception("The employee was not found in the system.");

                        item.Employee = employee;
                        employees.Add(employee);
                    }
                }

                SetRow(worksheet, item, row);
                row++;
            }
        }

        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);

        stream.Seek(0, SeekOrigin.Begin); ;
        return stream;
    }

    private static void SetHeaders(IXLWorksheet worksheet)
    {
        worksheet.Cell(1, 1).Value = "Inventory-Number";
        worksheet.Cell(1, 2).Value = "Registration-Number";
        worksheet.Cell(1, 3).Value = "Name";
        worksheet.Cell(1, 4).Value = "Price";
        worksheet.Cell(1, 5).Value = "Serial-Number";
        worksheet.Cell(1, 6).Value = "Date-Of-Purchase";
        worksheet.Cell(1, 7).Value = "Date-Of-Sale";
        worksheet.Cell(1, 8).Value = "Location:Building";
        worksheet.Cell(1, 9).Value = "Location:Room";
        worksheet.Cell(1, 10).Value = "Location:Additional-Note";
        worksheet.Cell(1, 11).Value = "Employee:Id";
        worksheet.Cell(1, 12).Value = "Employee:Name";
        worksheet.Cell(1, 13).Value = "Employee:Email";
        worksheet.Cell(1, 14).Value = "Description";
        worksheet.Cell(1, 15).Value = "Document-Number";
    }

    private static void ConfigureHeaders(IXLWorksheet worksheet)
    {
        worksheet.Range(1, 1, 1, 14).Style.Font.Bold = true;
    }

    private static void SetRow(IXLWorksheet worksheet, PropertyItemDto item, int row)
    {
        worksheet.Cell(row, 1).Value = item.InventoryNumber;
        worksheet.Cell(row, 2).Value = item.RegistrationNumber;
        worksheet.Cell(row, 3).Value = item.Name;
        worksheet.Cell(row, 4).Value = item.Price;
        worksheet.Cell(row, 5).Value = item.SerialNumber ?? "";
        worksheet.Cell(row, 6).Value = item.DateOfPurchase.ToString("yyyy-MM-dd");
        worksheet.Cell(row, 7).Value = item.DateOfSale?.ToString("yyyy-MM-dd") ?? "";
        worksheet.Cell(row, 8).Value = item.Location!.Building;
        worksheet.Cell(row, 9).Value = item.Location!.Room;
        worksheet.Cell(row, 10).Value = item.Location!.AdditionalNote ?? "";
        worksheet.Cell(row, 11).Value = item.Employee?.Id.ToString() ?? "";
        worksheet.Cell(row, 12).Value = item.Employee?.FullName ?? "";
        worksheet.Cell(row, 13).Value = item.Employee?.EmailAddress ?? "";
        worksheet.Cell(row, 14).Value = item.Description ?? "";
        worksheet.Cell(row, 15).Value = item.DocumentNumber ?? "";
    }

    private static Expression<Func<PropertyItem, PropertyItemDto>> PropertyItemDtoSelector
    {
        get
        {
            return i => new PropertyItemDto
            {
                Id = i.Id,
                PropertyId = i.PropertyId,
                EmployeeId = i.EmployeeId,
                InventoryNumber = i.InventoryNumber,
                RegistrationNumber = i.RegistrationNumber,
                Name = i.Name,
                Price = i.Price,
                SerialNumber = i.SerialNumber,
                DateOfPurchase = i.DateOfPurchase,
                DateOfSale = i.DateOfSale,
                Location = new PropertyItemLocationDto
                {
                    Room = i.Location.Room,
                    Building = i.Location.Building,
                    AdditionalNote = i.Location.AdditionalNote,
                },
                Description = i.Description,
                DocumentNumber = i.DocumentNumber,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt,
                LastCodeGeneratedAt = i.LastCodeGeneratedAt,
            };
        }
    }

    private static Expression<Func<Employee, EmployeeDto>> EmployeeDtoSelector
    {
        get
        {
            return e => new EmployeeDto
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                EmailAddress = e.EmailAddress,
            };
        }
    }
}
