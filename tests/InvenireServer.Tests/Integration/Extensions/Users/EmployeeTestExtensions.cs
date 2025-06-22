using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Extensions.Users;

public static class EmployeeTestExtensions
{
    public static RegisterEmployeeDto ToRegisterEmployeeDto(this Employee employee)
    {
        var dto = new RegisterEmployeeDto
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