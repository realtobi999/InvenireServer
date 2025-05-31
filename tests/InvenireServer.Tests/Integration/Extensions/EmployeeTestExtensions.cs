using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Dtos.Employees;

namespace InvenireServer.Tests.Integration.Extensions;

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
