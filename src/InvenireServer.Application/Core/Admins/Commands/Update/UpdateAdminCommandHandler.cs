using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateAdminCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(UpdateAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");

        admin.FirstName = request.FirstName;
        admin.LastName = request.LastName;

        await _repositories.Admins.ExecuteUpdateAsync(admin);
    }
}
