using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Create;

public class CreatePropertyItemsCommandHandler : IRequestHandler<CreatePropertyItemsCommand>
{
    private readonly IRepositoryManager _repositories;

    public CreatePropertyItemsCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(CreatePropertyItemsCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        // Preload all employees.
        var employees = new Dictionary<Guid, Employee>();
        foreach (var id in request.Items.Where(i => i.EmployeeId is not null).Select(i => i.EmployeeId!.Value).ToHashSet())
        {
            var employee = await _repositories.Employees.GetAsync(e => e.Id == id) ?? throw new NotFound404Exception($"The employee was not found in the system (key - {id}).");
            if (employee.OrganizationId != organization.Id) throw new BadRequest400Exception($"The employee isn't part of the organization (id - {employee.Id}).");
            employees[id] = employee;
        }

        var items = new List<PropertyItem>();
        foreach (var dto in request.Items)
        {
            var item = new PropertyItem
            {
                Id = dto.Id ?? Guid.NewGuid(),
                PropertyId = property.Id,
                EmployeeId = dto.EmployeeId,
                InventoryNumber = dto.InventoryNumber,
                RegistrationNumber = dto.RegistrationNumber,
                Name = dto.Name,
                Price = dto.Price,
                SerialNumber = dto.SerialNumber,
                DateOfPurchase = dto.DateOfPurchase,
                DateOfSale = dto.DateOfSale,
                Location = new PropertyItemLocation
                {
                    Room = dto.Location.Room,
                    Building = dto.Location.Building,
                    AdditionalNote = dto.Location.AdditionalNote
                },
                Description = dto.Description,
                DocumentNumber = dto.DocumentNumber,
                CreatedAt = DateTimeOffset.UtcNow,
                LastUpdatedAt = null
            };

            _repositories.Properties.Items.Create(item);
        }

        await _repositories.SaveOrThrowAsync();
    }
}