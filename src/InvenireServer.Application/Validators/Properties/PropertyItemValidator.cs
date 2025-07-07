using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators.Properties;

public class PropertyItemValidator : IEntityValidator<PropertyItem>
{
    private readonly IRepositoryManager _repositories;

    public PropertyItemValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(PropertyItem item)
    {
        // Date of sale must be later than date of purchase, if set.
        if (item.DateOfSale is not null && item.DateOfPurchase >= item.DateOfSale) return (false, new BadRequest400Exception($"{nameof(PropertyItem.DateOfSale)} must be later than {nameof(PropertyItem.DateOfPurchase)}."));

        // Date of purchase cannot be in the future.
        if (item.DateOfPurchase > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(PropertyItem.DateOfPurchase)} cannot be set in the future."));

        // Last update must be later than creation time, if set.
        if (item.LastUpdatedAt is not null && item.CreatedAt >= item.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(PropertyItem.LastUpdatedAt)} must be later than {nameof(PropertyItem.CreatedAt)}."));

        // Creation time cannot be set in the future.
        if (item.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(PropertyItem.CreatedAt)} cannot be set in the future."));

        // Property must be assigned.
        if (item.PropertyId is null) return (false, new BadRequest400Exception($"{nameof(Property)} must be assigned."));

        // Property must exist in the system.
        var property = await _repositories.Properties.GetAsync(p => p.Id == item.PropertyId);
        if (property is null) return (false, new NotFound404Exception($"The assigned {nameof(Property)} was not found in the system."));

        // Employee if set, must exist in the system.
        if (item.EmployeeId is not null)
            if (await _repositories.Employees.GetAsync(e => e.Id == item.EmployeeId) is null)
                return (false, new NotFound404Exception($"The assigned {nameof(Employee)} was not found in the system."));

        // Inventory number must be unique within the other property items.
        if (!await _repositories.Properties.Items.IsInventoryNumberUniqueAsync(item, property)) return (false, new BadRequest400Exception($"{nameof(PropertyItem.InventoryNumber)} must be unique."));

        // Registration number must be unique within the other property items.
        if (!await _repositories.Properties.Items.IsRegistrationNumberUniqueAsync(item, property)) return (false, new BadRequest400Exception($"{nameof(PropertyItem.RegistrationNumber)} must be unique."));

        // Document number must be unique within the other property items.
        if (!await _repositories.Properties.Items.IsDocumentNumberUniqueAsync(item, property)) return (false, new BadRequest400Exception($"{nameof(PropertyItem.DocumentNumber)} must be unique."));

        // Serial number if set, must be unique within the other property items.
        if (item.SerialNumber is not null)
            if (!await _repositories.Properties.Items.IsSerialNumberUniqueAsync(item, property))
                return (false, new BadRequest400Exception($"{nameof(PropertyItem.SerialNumber)} must be unique."));

        return (true, null);
    }
}