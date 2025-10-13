using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Employee.Remove;

public class RemoveEmployeeOrganizationCommandHandler : IRequestHandler<RemoveEmployeeOrganizationCommand>
{
    private readonly IRepositoryManager _repositories;

    public RemoveEmployeeOrganizationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(RemoveEmployeeOrganizationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var employee = await _repositories.Employees.GetAsync(e => e.Id == request.EmployeeId) ?? throw new NotFound404Exception("The employee was not found in the system.");

        if (employee.OrganizationId is null) throw new BadRequest400Exception("The employee isn't part of any organization");
        if (employee.OrganizationId != organization.Id) throw new Unauthorized401Exception("The employee isn't part of the organization");

        // If the organization owns a property, unassign employee's items from them.
        var property = await _repositories.Properties.GetForAsync(organization);
        if (property is not null)
        {
            var items = (await _repositories.Properties.Items.IndexAsync(new QueryOptions<PropertyItem, PropertyItem>
            {
                Filtering = new QueryFilteringOptions<PropertyItem>
                {
                    Filters =
                    [
                        i => i.PropertyId == property.Id && i.EmployeeId == employee.Id
                    ]
                }
            })).ToList();

            items.ForEach(item => item.EmployeeId = null);
            _repositories.Properties.Items.Update(items);
        }

        employee.OrganizationId = null;
        _repositories.Employees.Update(employee);

        await _repositories.SaveOrThrowAsync();
    }
}