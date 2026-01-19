using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

/// <summary>
/// Handler for the request to delete property items.
/// </summary>
public class DeletePropertyItemsCommandHandler : IRequestHandler<DeletePropertyItemsCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeletePropertyItemsCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to delete property items.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(DeletePropertyItemsCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        foreach (var id in request.Ids)
        {
            var item = await _repositories.Properties.Items.GetAsync(i => i.Id == id) ?? throw new NotFound404Exception($"The item was not found in the system. (key - {id})");

            if (item.PropertyId != property.Id) throw new BadRequest400Exception($"The item isn't part of the property. (key - {id})");

            _repositories.Properties.Items.Delete(item);
        }

        await _repositories.SaveOrThrowAsync();
    }
}