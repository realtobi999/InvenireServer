
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Remove;

public class RemoveOrganizationEmployeeCommandHandler : IRequestHandler<RemoveOrganizationEmployeeCommand>
{
    private readonly IServiceManager _services;

    public RemoveOrganizationEmployeeCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(RemoveOrganizationEmployeeCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);
        var employee = await _services.Employees.GetAsync(e => e.Id == request.EmployeeId);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization.");

        organization.RemoveEmployee(employee);

        await _services.Organizations.UpdateAsync(organization);
    }
}