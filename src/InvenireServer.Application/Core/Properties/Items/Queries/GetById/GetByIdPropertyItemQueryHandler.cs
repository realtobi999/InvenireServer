using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Dtos.Employees;

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
            Selector = PropertyItemDto.CoreSelector,
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
            Selector = EmployeeDto.BaseSelector,
            Filtering = new QueryFilteringOptions<Employee>
            {
                Filters = [e => e.Id == item.EmployeeId]
            },
        });

        return item;
    }
}
