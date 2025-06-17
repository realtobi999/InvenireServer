using InvenireServer.Domain.Entities;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Interfaces.Factories.Organizations;

public interface IOrganizationFactory
{
    Organization Create(CreateOrganizationDto dto);
}
