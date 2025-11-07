using System.Linq.Expressions;
using System.Text.Json;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToJson;

public class ExportToJsonPropertyItemQueryHandler : IRequestHandler<ExportToJsonPropertyItemQuery, Stream>
{
    private readonly IRepositoryManager _repositories;

    public ExportToJsonPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<Stream> Handle(ExportToJsonPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        var limit = QueryPaginationOptions.MAX_LIMIT;
        var employees = new List<EmployeeDto>();

        writer.WriteStartArray();
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

                JsonSerializer.Serialize(writer, item);
            }
        }
        writer.WriteEndArray();
        writer.Flush();

        stream.Position = 0;
        return stream;
    }

    private static Expression<Func<PropertyItem, PropertyItemDto>> PropertyItemDtoSelector
    {
        get
        {
            return i => new PropertyItemDto
            {
                Id = i.Id,
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
