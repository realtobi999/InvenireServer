using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Scan;

public class ScanPropertyItemCommandHandler : IRequestHandler<ScanPropertyItemCommand>
{
    private readonly IRepositoryManager _repositories;

    public ScanPropertyItemCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(ScanPropertyItemCommand request, CancellationToken ct)
    {
        var item = await _repositories.Properties.Items.GetAsync(i => i.Id == request.ItemId) ?? throw new NotFound404Exception("The item was not found in the system.");

        var scan = request.Jwt.GetRole() switch
        {
            Jwt.Roles.EMPLOYEE => await GetScanAsEmployeeAsync(request.Jwt, item),
            Jwt.Roles.ADMIN => await GetScanAsAdminAsync(request.Jwt, item),
            _ => throw new Unauthorized401Exception(),
        };

        await _repositories.Properties.Items.ScanAsync(item, scan);
        await _repositories.Properties.Scans.ExecuteUpdateAsync(scan);
    }

    private async Task<PropertyScan> GetScanAsEmployeeAsync(Jwt jwt, PropertyItem item)
    {
        var employee = await _repositories.Employees.GetAsync(jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
        var scan = await _repositories.Properties.Scans.GetInProgressForAsync(property) ?? throw new BadRequest400Exception("The organization doesn't have an active scan.");

        if (item.EmployeeId != employee.Id) throw new Unauthorized401Exception("The item doesn't belong to the item.");
        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't part of the property.");

        return scan;

    }

    private async Task<PropertyScan> GetScanAsAdminAsync(Jwt jwt, PropertyItem item)
    {
        var admin = await _repositories.Admins.GetAsync(jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
        var scan = await _repositories.Properties.Scans.GetInProgressForAsync(property) ?? throw new BadRequest400Exception("The organization doesn't have an active scan.");

        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't part of the property.");

        return scan;
    }
}
