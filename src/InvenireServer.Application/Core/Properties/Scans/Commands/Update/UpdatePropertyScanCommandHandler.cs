
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Update;

public class UpdatePropertyScanCommandHandler : IRequestHandler<UpdatePropertyScanCommand>
{
    private readonly IServiceManager _services;

    public UpdatePropertyScanCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdatePropertyScanCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own an organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");
        var scan = await _services.Properties.Scans.TryGetInProgressForAsync(property) ?? throw new BadRequest400Exception("There are currently no active scans.");

        scan.Name = request.Name;
        scan.Description = request.Description;

        await _services.Properties.Scans.UpdateAsync(scan);
    }
}