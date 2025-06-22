using System.Linq.Expressions;
using System.Security.Claims;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Employees;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailManager _email;
    private readonly IJwtManager _jwt;
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<Employee> _validator;

    public EmployeeService(
        IRepositoryManager repositories,
        IFactoryManager factories,
        IEmailManager email,
        IJwtManager jwt,
        IConfiguration configuration)
    {
        _jwt = jwt;
        _email = email;
        _validator = factories.Validators.Initiate<Employee>();
        _repositories = repositories;
        _configuration = configuration;
    }

    public async Task<Employee> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    public async Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        var employee = await _repositories.Employees.GetAsync(predicate);

        if (employee is null) throw new NotFound404Exception($"The requested {nameof(Employee).ToLower()} was not found in the system.");

        return employee;
    }

    public async Task CreateAsync(Employee employee)
    {
        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Create(employee);
        await _repositories.SaveOrThrowAsync();
    }

    public Jwt CreateJwt(Employee employee)
    {
        return _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", employee.IsVerified ? bool.TrueString : bool.FalseString)
        ]);
    }

    public async Task UpdateAsync(Employee employee)
    {
        employee.LastUpdatedAt = DateTimeOffset.UtcNow;

        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Update(employee);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task SendVerificationEmailAsync(Employee employee)
    {
        var jwt = CreateJwt(employee);
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var dto = new EmployeeVerificationEmailDto
        {
            EmployeeName = employee.Name,
            EmployeeAddress = employee.EmailAddress,
            VerificationLink = $"{_configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };

        var email = _email.Builders.Employee.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);
    }

    public async Task ConfirmEmailVerificationAsync(Employee employee)
    {
        if (employee.IsVerified) throw new BadRequest400Exception("Email is already verified.");

        employee.IsVerified = true;
        await UpdateAsync(employee);
    }
}