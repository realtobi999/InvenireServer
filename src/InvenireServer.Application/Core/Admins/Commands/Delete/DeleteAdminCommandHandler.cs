using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Commands.Delete;

public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand>
{
    private readonly IServiceManager _services;

    public DeleteAdminCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeleteAdminCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);

        if (await _services.Organizations.TryGetForAsync(admin) is not null) throw new BadRequest400Exception("The organization must be deleted before the admin can be removed.");

        await _services.Admins.DeleteAsync(admin);
    }
}
