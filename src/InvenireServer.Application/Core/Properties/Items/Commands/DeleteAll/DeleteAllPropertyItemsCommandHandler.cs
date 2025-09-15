using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Items.Commands.DeleteAll;

public class DeleteAllPropertyItemsCommandHandler : IRequestHandler<DeleteAllPropertyItemsCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteAllPropertyItemsCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(DeleteAllPropertyItemsCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        await _repositories.Properties.Items.ExecuteDeleteWhereAsync(i => i.PropertyId == property.Id);
    }
}
