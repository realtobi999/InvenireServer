using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Complete;

public class CompletePropertyScanCommandHandler : IRequestHandler<CompletePropertyScanCommand>
{
    private readonly IRepositoryManager _repositories;

    public CompletePropertyScanCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(CompletePropertyScanCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
        var scan = await _repositories.Properties.Scans.GetInProgressForAsync(property) ?? throw new BadRequest400Exception("The organization doesn't have an active scan.");

        scan.Status = PropertyScanStatus.COMPLETED;
        scan.CompletedAt = DateTimeOffset.UtcNow;

        _repositories.Properties.Scans.Update(scan);

        await _repositories.SaveOrThrowAsync();
    }
}
