
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand>
{
    private readonly IServiceManager _services;

    public UpdateAdminCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdateAdminCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);

        admin.Name = admin.Name;

        await _services.Admins.UpdateAsync(admin);
    }
}
