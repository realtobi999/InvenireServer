using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators.Properties;

public class PropertyValidator : IValidator<Property>
{
    private readonly IRepositoryManager _repositories;

    public PropertyValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Property property)
    {
        // Last update must be later than creation time, if set.
        if (property.LastUpdatedAt is not null && property.CreatedAt >= property.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(Property.LastUpdatedAt)} must be later than CreatedAt."));

        // Creation time cannot be set in the future.
        if (property.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(Property.CreatedAt)} cannot be set in the future."));

        // Organization must be assigned.
        if (property.OrganizationId is null) return (false, new BadRequest400Exception($"{nameof(Organization)}) must be assigned."));

        // Organization must exist in the system
        var organization = await _repositories.Organizations.GetAsync(o => o.Id == property.OrganizationId);
        if (organization is null) return (false, new NotFound404Exception($"The assigned {nameof(Organization)} was not found in the system."));

        // Items if set, must exist in the system.
        if (property.Items.Count != 0)
            foreach (var item in property.Items)
                if (await _repositories.Properties.Items.GetAsync(i => i.Id == item.Id) is null)
                    return (false, new NotFound404Exception($"The assigned {nameof(PropertyItem)} was not found in the system."));

        return (true, null);
    }
}