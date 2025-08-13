using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Extensions.Users;

public static class EmployeeTestExtensions
{
    public static RegisterEmployeeCommand ToRegisterEmployeeCommand(this Employee employee)
    {
        var dto = new RegisterEmployeeCommand
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
            PasswordConfirm = employee.Password
        };

        return dto;
    }

    public static void SetAsVerified(this Employee employee, InvenireServerContext context)
    {
        context.Employees.FirstOrDefault(e => e.Id == employee.Id)!.Verify();
        context.SaveChanges();
    }
}