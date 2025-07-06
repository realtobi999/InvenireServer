using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Integration.Extensions.Organizations;

public static class OrganizationTextExtensions
{
    public static CreateOrganizationCommand ToCreateOrganizationCommand(this Organization organization)
    {
        var dto = new CreateOrganizationCommand
        {
            Id = organization.Id,
            Name = organization.Name,
            Jwt = null,
            FrontendBaseUrl = null
        };

        return dto;
    }

    public static void AddEmployee(this Organization organization, Employee employee, InvenireServerContext context)
    {
        context.Organizations.FirstOrDefault(o => o.Id == organization.Id)!.AddEmployee(employee);
        context.SaveChanges();
    }
}