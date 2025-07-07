
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
        // Validate the request.
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var property = await _services.Properties.GetAsync(p => p.Id == request.PropertyId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        if (admin.OrganizationId != organization.Id) throw new Unauthorized401Exception();
        if (property.OrganizationId != organization.Id) throw new BadRequest400Exception("This property doesnt belong to your organization.");

        // Preload all the items.
        var items = new List<PropertyItem>();
        foreach (var id in request.ItemIds)
        {
            var item = await _services.Properties.Items.GetAsync(i => i.Id == id);
            if (item.PropertyId != property.Id) throw new BadRequest400Exception("Cannot update a item from a property you do not own.");
            items.Add(item);
        }

        // Delete the items.
        await _services.Properties.Items.DeleteAsync(items);
    }
}
