
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Create;

public class CreatePropertyItemsCommandHandler : IRequestHandler<CreatePropertyItemsCommand>
{
    private readonly IServiceManager _services;

    public CreatePropertyItemsCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(CreatePropertyItemsCommand request, CancellationToken _)
    {
        // Validate the request.
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var property = await _services.Properties.GetAsync(p => p.Id == request.PropertyId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        if (admin.Id != organization.Admin!.Id) throw new Unauthorized401Exception();
        if (property.Id != organization.Property!.Id) throw new BadRequest400Exception("This property doesnt belong to your organization.");

        // Preload all employees.
        var employees = new Dictionary<Guid, Employee>();
        foreach (var id in request.Items.Where(i => i.EmployeeId is not null).Select(i => i.EmployeeId!.Value).ToHashSet())
        {
            employees[id] = await _services.Employees.GetAsync(e => e.Id == id);
        }

        // Initialize all the items.
        var items = new List<PropertyItem>();
        foreach (var dto in request.Items)
        {
            var item = new PropertyItem
            {
                Id = dto.Id ?? Guid.NewGuid(),
                InventoryNumber = dto.InventoryNumber,
                RegistrationNumber = dto.RegistrationNumber,
                Name = dto.Name,
                Price = dto.Price,
                SerialNumber = dto.SerialNumber,
                DateOfPurchase = dto.DateOfPurchase,
                DateOfSale = dto.DateOfSale,
                Description = dto.Description,
                DocumentNumber = dto.DocumentNumber,
                CreatedAt = DateTimeOffset.UtcNow,
                LastUpdatedAt = null,
            };

            if (dto.EmployeeId is not null && employees.TryGetValue(dto.EmployeeId.Value, out var employee))
            {
                employee.AddItem(item);
            }

            items.Add(item);
            property.AddItem(item);
        }

        // Save changes to the database.
        await _services.Properties.Items.CreateAsync(items);
        if (employees.Values.Count != 0) await _services.Employees.UpdateAsync(employees.Values);
    }
}
