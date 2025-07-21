
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Commands.Update;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand>
{
    private readonly IServiceManager _services;

    public UpdatePropertyCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdatePropertyCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");

        await _services.Properties.UpdateAsync(property);
    }
}
