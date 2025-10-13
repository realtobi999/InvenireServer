using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Queries.GetById;

public class GetByIdPropertyItemQueryHandler : IRequestHandler<GetByIdPropertyItemQuery, PropertyItemDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertyItemDto> Handle(GetByIdPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var item = await _repositories.Properties.Items.GetAsync(new QueryOptions<PropertyItem, PropertyItemDto>
        {
            Selector = PropertyItemDtoSelector,
            Filtering = new QueryFilteringOptions<PropertyItem>
            {
                Filters =
                [
                    i => i.PropertyId == property.Id && i.Id == request.ItemId
                ],
            }
        }) ?? throw new NotFound404Exception("The item was not found in the system.");

        item.Employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
        {
            Selector = EmployeeDtoSelector,
            Filtering = new QueryFilteringOptions<Employee>
            {
                Filters = [e => e.Id == item.EmployeeId]
            },
        });

        return item;
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
                OrganizationId = e.OrganizationId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = $"{e.FirstName} {e.LastName}",
                EmailAddress = e.EmailAddress,
                CreatedAt = e.CreatedAt,
                LastUpdatedAt = e.LastUpdatedAt,
            };
        }
    }
}
