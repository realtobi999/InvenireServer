using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Update;

public class UpdatePropertyItemsCommandHandler : IRequestHandler<UpdatePropertyItemsCommand>
{
    private readonly IServiceManager _services;

    public UpdatePropertyItemsCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdatePropertyItemsCommand request, CancellationToken _)
    {
        // Validate the request.
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var property = await _services.Properties.GetAsync(p => p.Id == request.PropertyId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        if (admin.OrganizationId != organization.Id) throw new Unauthorized401Exception();

        if (property.OrganizationId != organization.Id) throw new BadRequest400Exception("This property doesnt belong to your organization.");

        // Preload all the items.
        var items = new Dictionary<PropertyItem, UpdatePropertyItemCommand>();
        foreach (var id in request.Items.Select(i => i.Id))
        {
            var item = await _services.Properties.Items.GetAsync(i => i.Id == id);
            if (item.PropertyId != property.Id) throw new BadRequest400Exception("Cannot update a item from a property you do not own.");

            items[item] = request.Items.FirstOrDefault(i => i.Id == item.Id)!;
        }

        // Preload only employees involved in changed assignments
        var employees = new Dictionary<Guid, Employee>();
        foreach (var id in items.Where(pair => pair.Key.EmployeeId != pair.Value.EmployeeId).SelectMany(pair => new[] { pair.Key.EmployeeId, pair.Value.EmployeeId }).OfType<Guid>().ToHashSet())
        {
            var employee = await _services.Employees.GetAsync(e => e.Id == id);
            if (employee.OrganizationId != organization.Id)
                throw new BadRequest400Exception("Cannot assign property item to an employee from a another organization.");

            employees[id] = employee;
        }

        // Update items and handle employee reassignment.
        foreach (var (item, command) in items)
        {
            item.InventoryNumber = command.InventoryNumber;
            item.RegistrationNumber = command.RegistrationNumber;
            item.Name = command.Name;
            item.Price = command.Price;
            item.SerialNumber = command.SerialNumber;
            item.DateOfPurchase = command.DateOfPurchase;
            item.DateOfSale = command.DateOfSale;
            item.Description = command.Description;
            item.DocumentNumber = command.DocumentNumber;

            if (item.EmployeeId != command.EmployeeId)
            {
                if (item.EmployeeId.HasValue && employees.TryGetValue(item.EmployeeId.Value, out var originalEmployee)) originalEmployee.RemoveItem(item);

                if (command.EmployeeId.HasValue && employees.TryGetValue(command.EmployeeId.Value, out var newEmployee)) newEmployee.AddItem(item);
            }
        }

        // Save changes to the database.
        await _services.Properties.Items.UpdateAsync(items.Keys);
        if (employees.Values.Count != 0) await _services.Employees.UpdateAsync(employees.Values);
    }
}
