using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Extensions.Organizations;

/// <summary>
/// Provides test extensions for <see cref="Organization"/>.
/// </summary>
public static class OrganizationTextExtensions
{
    /// <summary>
    /// Creates a <see cref="CreateOrganizationCommand"/> from an organization instance.
    /// </summary>
    /// <param name="organization">Source organization.</param>
    /// <returns>Create organization command.</returns>
    public static CreateOrganizationCommand ToCreateOrganizationCommand(this Organization organization)
    {
        var dto = new CreateOrganizationCommand
        {
            Id = organization.Id,
            Name = organization.Name
        };

        return dto;
    }

    /// <summary>
    /// Adds an employee to the organization in the database.
    /// </summary>
    /// <param name="organization">Organization to update.</param>
    /// <param name="employee">Employee to add.</param>
    /// <param name="context">Database context to update.</param>
    public static void AddEmployee(this Organization organization, Employee employee, InvenireServerContext context)
    {
        context.Organizations.FirstOrDefault(o => o.Id == organization.Id)!.AddEmployee(context.Employees.FirstOrDefault(e => e.Id == employee.Id)!);
        context.SaveChanges();
    }
}
