using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

public class CreatePropertyScanCommandHandler : IRequestHandler<CreatePropertyScanCommand, CreatePropertyScanCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreatePropertyScanCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<CreatePropertyScanCommandResult> Handle(CreatePropertyScanCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if ((await _repositories.Properties.Scans.IndexInProgressForAsync(property)).Any()) throw new Conflict409Exception("The organization already has an active scan.");

        var scan = new PropertyScan
        {
            Id = request.Id ?? Guid.NewGuid(),
            PropertyId = property.Id,
            Name = request.Name,
            Description = request.Description,
            Status = PropertyScanStatus.IN_PROGRESS,
            CreatedAt = DateTimeOffset.UtcNow,
            CompletedAt = null,
            LastUpdatedAt = null,
        };

        await _repositories.Properties.Scans.ExecuteCreateAsync(scan);
        await _repositories.Properties.Scans.RegisterItemsAsync(scan);

        return new CreatePropertyScanCommandResult
        {
            Scan = scan
        };
    }
}
