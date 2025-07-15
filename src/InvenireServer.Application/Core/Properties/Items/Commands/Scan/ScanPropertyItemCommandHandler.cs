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

        switch (request.Jwt.GetRole())
        {
            case Jwt.Roles.EMPLOYEE:
                await HandleAsEmployee(request, item);
                break;
            case Jwt.Roles.ADMIN:
                await HandleAsAdmin(request, item);
                break;
            default:
                throw new Unauthorized401Exception();
        }
    }

    private async Task HandleAsEmployee(ScanPropertyItemCommand request, PropertyItem item)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId) ?? throw new BadRequest400Exception("You are not part of an organization.");
        var property = await _services.Properties.TryGetAsync(p => p.OrganizationId == organization.Id) ?? throw new BadRequest400Exception("Organization you are part of doesn't have a property.");
        var scan = await _services.Properties.Scans.TryGetAsync(s => !s.ClosedAt.HasValue) ?? throw new BadRequest400Exception("There is currently no active scans.");

        if (item.EmployeeId != employee.Id) throw new Unauthorized401Exception();
        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't a part of the property.");

        scan.ScannedItems.Add(item);

        await _services.Properties.Scans.UpdateAsync(scan);
    }

    private async Task HandleAsAdmin(ScanPropertyItemCommand request, PropertyItem item)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetAsync(p => p.OrganizationId == organization.Id) ?? throw new BadRequest400Exception("You have not created a property.");
        var scan = await _services.Properties.Scans.TryGetAsync(s => !s.ClosedAt.HasValue) ?? throw new BadRequest400Exception("There is currently no active scans.");

        if (item.PropertyId != property.Id) throw new BadRequest400Exception("The item isn't a part of your property.");

        scan.ScannedItems.Add(item);

        await _services.Properties.Scans.UpdateAsync(scan);
    }
}
