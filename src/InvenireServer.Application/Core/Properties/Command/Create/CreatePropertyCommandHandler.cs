using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Command.Create;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, CreatePropertyCommandResponse>
{
    private readonly IServiceManager _services;

    public CreatePropertyCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<CreatePropertyCommandResponse> Handle(CreatePropertyCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        // Ensure the admin is the owner of the organization.
        if (admin.Id != organization.Admin!.Id) throw new Unauthorized401Exception();

        // Create the property and assign it to the organization.
        var property = new Property
        {
            Id = request.Id ?? Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
        };
        organization.AssignProperty(property);

        // Save the changes.
        await _services.Properties.CreateAsync(property);
        await _services.Organizations.UpdateAsync(organization);

        return new CreatePropertyCommandResponse
        {
            Property = property,
        };
    }
}
