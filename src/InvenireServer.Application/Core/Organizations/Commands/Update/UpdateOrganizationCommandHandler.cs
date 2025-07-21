using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Update;

public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand>
{
    private readonly IServiceManager _services;

    public UpdateOrganizationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdateOrganizationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization.");

        organization.Name = request.Name;

        await _services.Organizations.UpdateAsync(organization);
    }
}
