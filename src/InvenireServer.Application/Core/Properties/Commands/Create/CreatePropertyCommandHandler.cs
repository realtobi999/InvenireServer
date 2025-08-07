using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, CreatePropertyCommandResponse>
{
    private readonly IServiceManager _services;

    public CreatePropertyCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<CreatePropertyCommandResponse> Handle(CreatePropertyCommand request, CancellationToken ct)
    {
        // Get the admin and his organization.
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization. You must first create an organization before adding a property.");

        // Create the property and assign it to the organization.
        var property = new Property
        {
            Id = request.Id ?? Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null
        };
        organization.AssignProperty(property);

        // Save the changes.
        await _services.Properties.CreateAsync(property);
        await _services.Organizations.UpdateAsync(organization);

        return new CreatePropertyCommandResponse
        {
            Property = property
        };
    }
}