using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Scan;

public class ScanPropertyItemCommandHandler : IRequestHandler<ScanPropertyItemCommand>
{
    private readonly IServiceManager _services;

    public ScanPropertyItemCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(ScanPropertyItemCommand request, CancellationToken _)
    {
        var item = await _services.Properties.Items.GetAsync(i => i.Id == request.ItemId);

        var scan = (PropertyScan?)null;
        scan = request.Jwt.GetRole() switch
        {
            Jwt.Roles.EMPLOYEE => await GetScanAsEmployeeAsync(request.Jwt, item),
            Jwt.Roles.ADMIN => await GetScanAsAdminAsync(request.Jwt, item),
            _ => throw new Unauthorized401Exception(),
        };

        scan.ScannedItems.Add(item);

        await _services.Properties.Scans.UpdateAsync(scan);
    }

    private async Task<PropertyScan> GetScanAsEmployeeAsync(Jwt jwt, PropertyItem item)
    {
        var employee = await _services.Employees.GetAsync(jwt);
        var organization = await _services.Organizations.TryGetForAsync(employee) ?? throw new BadRequest400Exception("You are not part of an organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("Organization you are part of doesn't have a property.");
        var scan = await _services.Properties.Scans.TryGetInProgressForAsync(property) ?? throw new BadRequest400Exception("There are currently no active scans.");

        if (item.EmployeeId != employee.Id) throw new Unauthorized401Exception();
        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't a part of the property.");

        return scan;

    }

    private async Task<PropertyScan> GetScanAsAdminAsync(Jwt jwt, PropertyItem item)
    {
        var admin = await _services.Admins.GetAsync(jwt);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");
        var scan = await _services.Properties.Scans.TryGetInProgressForAsync(property) ?? throw new BadRequest400Exception("There are currently no active scans.");

        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't a part of your property.");

        return scan;
    }
}
