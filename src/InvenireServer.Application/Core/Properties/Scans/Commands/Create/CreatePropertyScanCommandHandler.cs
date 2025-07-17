using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

public class CreatePropertyScanCommandHandler : IRequestHandler<CreatePropertyScanCommand, CreatePropertyScanCommandResult>
{
    private readonly IServiceManager _services;

    public CreatePropertyScanCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<CreatePropertyScanCommandResult> Handle(CreatePropertyScanCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetAsync(p => p.OrganizationId == organization.Id) ?? throw new BadRequest400Exception("You have not created a property.");

        if ((await _services.Properties.Scans.IndexInProgressAsync(property)).Any()) throw new Conflict409Exception("An active property scan is already created.");

        var scan = new PropertyScan
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Status = PropertyScanStatus.IN_PROGRESS,
            CreatedAt = DateTimeOffset.UtcNow,
            CompletedAt = null,
            LastUpdatedAt = null,
            PropertyId = property.Id,
        };

        await _services.Properties.Scans.CreateAsync(scan);

        return new CreatePropertyScanCommandResult
        {
            Scan = scan
        };
    }
}
