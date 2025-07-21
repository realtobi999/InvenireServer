
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Commands.Delete;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand>
{
    private readonly IServiceManager _services;

    public DeletePropertyCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeletePropertyCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");

        await _services.Properties.DeleteAsync(property);
    }
}
