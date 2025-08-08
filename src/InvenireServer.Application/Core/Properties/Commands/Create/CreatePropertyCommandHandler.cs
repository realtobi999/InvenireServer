using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, CreatePropertyCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreatePropertyCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<CreatePropertyCommandResult> Handle(CreatePropertyCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        if (await _repositories.Properties.GetForAsync(organization) is not null)
            throw new Conflict409Exception("The organization already has a property.");

        var property = new Property
        {
            Id = request.Id ?? Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null
        };
        organization.AssignProperty(property);

        _repositories.Properties.Create(property);
        _repositories.Organizations.Update(organization);

        await _repositories.SaveOrThrowAsync();

        return new CreatePropertyCommandResult
        {
            Property = property
        };
    }
}