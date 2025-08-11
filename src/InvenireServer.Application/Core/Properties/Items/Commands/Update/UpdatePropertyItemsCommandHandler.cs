using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Update;

public class UpdatePropertyItemsCommandHandler : IRequestHandler<UpdatePropertyItemsCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdatePropertyItemsCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(UpdatePropertyItemsCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        // Preload all the items.
        var items = new Dictionary<PropertyItem, UpdatePropertyItemCommand>();
        foreach (var id in request.Items.Select(i => i.Id))
        {
            var item = await _repositories.Properties.Items.GetAsync(i => i.Id == id) ?? throw new NotFound404Exception($"The item was not found in the system (key - {id}).");
            if (item.PropertyId != property.Id) throw new BadRequest400Exception($"The item isn't part of the organization (id - {item.Id}).");
            items[item] = request.Items.FirstOrDefault(i => i.Id == item.Id)!;
        }

        // Preload only employees involved in changed assignments.
        var employees = new Dictionary<Guid, Employee>();
        foreach (var id in items.Where(pair => pair.Key.EmployeeId != pair.Value.EmployeeId).SelectMany(pair => new[] { pair.Key.EmployeeId, pair.Value.EmployeeId }).OfType<Guid>().ToHashSet())
        {
            var employee = await _repositories.Employees.GetAsync(e => e.Id == id) ?? throw new NotFound404Exception($"The employee was not found in the system (key - {id}).");
            if (employee.OrganizationId != organization.Id) throw new BadRequest400Exception($"The employee isn't part of the organization (id - {employee.Id}).");
            employees[id] = employee;
        }

        foreach (var (item, command) in items)
        {
            item.InventoryNumber = command.InventoryNumber;
            item.RegistrationNumber = command.RegistrationNumber;
            item.Name = command.Name;
            item.Price = command.Price;
            item.SerialNumber = command.SerialNumber;
            item.DateOfPurchase = command.DateOfPurchase;
            item.DateOfSale = command.DateOfSale;
            item.Location.Room = command.Location.Room;
            item.Location.Building = command.Location.Building;
            item.Location.AdditionalNote = command.Location.AdditionalNote;
            item.Description = command.Description;
            item.DocumentNumber = command.DocumentNumber;
            item.EmployeeId = command.EmployeeId;

            _repositories.Properties.Items.Update(item);
        }

        await _repositories.SaveOrThrowAsync();
    }
}