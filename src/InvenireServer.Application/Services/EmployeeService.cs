using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Factories;
using InvenireServer.Domain.Core.Interfaces.Services;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;

namespace InvenireServer.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IJwtFactory _jwt;
    private readonly IEmailManager _email;
    private readonly IValidator<Employee> _validator;
    private readonly IRepositoryManager _repositories;

    public EmployeeService(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email)
    {
        _jwt = factories.Jwt;
        _email = email;
        _validator = factories.Validators.Initiate<Employee>();
        _repositories = repositories;
    }

    public async Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        var employee = await _repositories.Employees.GetAsync(predicate);

        if (employee is null)
        {
            throw new NotFound404Exception(nameof(employee));
        }

        return employee;
    }

    public async Task CreateAsync(Employee employee)
    {
        // Validate.
        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Create(employee);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task SendVerificationEmailAsync(Employee employee, HttpRequest request)
    {
        var jwt = _jwt.Create([
            new("role", nameof(Employee).ToUpper()),
            new("employee_id", employee.Id.ToString()),
            new("email_verification", bool.TrueString)
        ]);

        var dto = new EmployeeVerificationEmailDto
        {
            EmployeeAddress = employee.EmailAddress,
            EmployeeName = employee.Name,
            VerificationLink = $"{request.Scheme}://{request.Host}/api/auth/employee/verify-email-verification?token={jwt.Write()}"
        };
        var message = _email.EmployeeBuilder.BuildVerificationEmail(dto);

        await _email.Sender.SendEmailAsync(message);
    }

    public Task AcceptVerificationTokenAsync()
    {
        throw new NotImplementedException();
    }
}
