
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Delete;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
{
    private readonly IServiceManager _services;

    public DeleteOrganizationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeleteOrganizationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization.");

        await _services.Organizations.DeleteAsync(organization);
    }
}
