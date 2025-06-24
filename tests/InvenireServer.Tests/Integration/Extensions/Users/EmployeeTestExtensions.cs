using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Extensions.Users;

public static class EmployeeTestExtensions
{
    public static RegisterEmployeeCommand ToRegisterEmployeeCommand(this Employee employee)
    {
        var dto = new RegisterEmployeeCommand
        {
            Id = employee.Id,
            Name = employee.Name,
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
            PasswordConfirm = employee.Password
        };

        return dto;
    }
}