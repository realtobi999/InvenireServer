using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Update;

/// <summary>
/// Handler for the request to update a property scan.
/// </summary>
public class UpdatePropertyScanCommandHandler : IRequestHandler<UpdatePropertyScanCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdatePropertyScanCommandHandler(IRepositoryManager services)
    {
        _repositories = services;
    }

    /// <summary>
    /// Handles the request to update a property scan.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(UpdatePropertyScanCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
        var scan = await _repositories.Properties.Scans.GetInProgressForAsync(property) ?? throw new BadRequest400Exception("The organization doesn't have an active scan.");

        scan.Name = request.Name;
        scan.Description = request.Description;

        await _repositories.Properties.Scans.ExecuteUpdateAsync(scan);
    }
}