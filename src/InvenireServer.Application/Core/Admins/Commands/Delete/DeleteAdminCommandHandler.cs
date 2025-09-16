using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Delete;

public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteAdminCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");

        if (await _repositories.Organizations.GetForAsync(admin) is not null) throw new BadRequest400Exception("The admin's organization must be deleted before the admin can be removed.");

        await _repositories.Admins.ExecuteDeleteAsync(admin);
    }
}
