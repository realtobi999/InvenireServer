using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Services;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Factories;
using InvenireServer.Domain.Core.Dtos.Employees.Emails;

namespace InvenireServer.Application.Services;

/// <summary>
/// Provides operations related to employee account management.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IJwtFactory _jwt;
    private readonly IEmailManager _email;
    private readonly IValidator<Employee> _validator;
    private readonly IRepositoryManager _repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeService"/> class.
    /// </summary>
    /// <param name="repositories">Repository access manager for employee data.</param>
    /// <param name="factories">Factory manager for resolving validators and JWT utilities.</param>
    /// <param name="email">Email manager used for sending verification emails.</param>
    public EmployeeService(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email)
    {
        _jwt = factories.Jwt;
        _email = email;
        _validator = factories.Validators.Initiate<Employee>();
        _repositories = repositories;
    }

    /// <summary>
    /// Retrieves an employee based on the 'employee_id' claim in the provided JWT.
    /// </summary>
    /// <param name="jwt">The JWT containing the employee claim.</param>
    /// <returns>The matching <see cref="Employee"/> entity.</returns>
    /// <exception cref="BadRequest400Exception">
    /// Thrown if the claim is missing or improperly formatted.
    /// </exception>
    public async Task<Employee> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null)
        {
            throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");
        }

        if (!Guid.TryParse(claim.Value, out var id))
        {
            throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");
        }

        return await GetAsync(e => e.Id == id);
    }

    /// <summary>
    /// Retrieves an employee entity matching the specified predicate.
    /// </summary>
    /// <param name="predicate">Condition used to locate the employee.</param>
    /// <returns>The matching <see cref="Employee"/> entity.</returns>
    /// <exception cref="NotFound404Exception">Thrown if no matching employee is found.</exception>
    public async Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        var employee = await _repositories.Employees.GetAsync(predicate);

        if (employee is null)
        {
            throw new NotFound404Exception(nameof(employee));
        }

        return employee;
    }

    /// <summary>
    /// Validates and creates a new employee in the database.
    /// </summary>
    /// <param name="employee">The employee entity to be created.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CreateAsync(Employee employee)
    {
        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Create(employee);
        await _repositories.SaveOrThrowAsync();
    }

    /// <summary>
    /// Validates and updates an existing employee in the database.
    /// </summary>
    /// <param name="employee">The employee entity with updated data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateAsync(Employee employee)
    {
        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Update(employee);
        await _repositories.SaveOrThrowAsync();
    }

    /// <summary>
    /// Sends an email verification link to the specified employee.
    /// </summary>
    /// <param name="employee">The employee to whom the email should be sent.</param>
    /// <param name="request">The current HTTP request, used to build the callback URL.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendEmailVerificationAsync(Employee employee, HttpRequest request)
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
            VerificationLink = $"{request.Scheme}://{request.Host}/api/auth/employee/email-verification/confirm?token={jwt.Write()}"
        };
        var message = _email.EmployeeBuilder.BuildVerificationEmail(dto);

        await _email.Sender.SendEmailAsync(message);
    }

    /// <summary>
    /// Marks the employee's email address as verified.
    /// </summary>
    /// <param name="employee">The employee whose email verification status will be updated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ConfirmEmailVerificationAsync(Employee employee)
    {
        employee.IsEmailAddressVerified = true;
        await UpdateAsync(employee);
    }
}
