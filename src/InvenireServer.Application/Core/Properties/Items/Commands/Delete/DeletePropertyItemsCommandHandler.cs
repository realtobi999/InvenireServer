using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

public class DeletePropertyItemsCommandHandler : IRequestHandler<DeletePropertyItemsCommand>
{
    private readonly IServiceManager _services;

    public DeletePropertyItemsCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeletePropertyItemsCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization. You must create an organization before modifying your property.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property. You must create a property before modifying its items.");

        // Preload all the items.
        var items = new List<PropertyItem>();
        foreach (var id in request.Ids)
        {
            var item = await _services.Properties.Items.GetAsync(i => i.Id == id);
            if (item.PropertyId != property.Id) throw new BadRequest400Exception("Cannot update a item from a property you do not own.");
            items.Add(item);
        }

        property.RemoveItems(items);

        // Save changes to the database.
        await _services.Properties.Items.DeleteAsync(items);
        await _services.Properties.UpdateAsync(property);
    }
}