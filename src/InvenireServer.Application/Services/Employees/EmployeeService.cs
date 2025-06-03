using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees.Emails;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services.Employees;

/// <summary>
/// Provides operations related to employee account management.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IConfiguration _configuration;
    private readonly IValidator<Employee> _validator;
    private readonly IRepositoryManager _repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeService"/> class.
    /// </summary>
    /// <param name="repositories">The repository manager for data access operations.</param>
    /// <param name="factories">The factory manager for creating validators and other services.</param>
    /// <param name="email">The email manager for sending emails.</param>
    /// <param name="jwt">The JWT manager for token operations.</param>
    /// <param name="configuration">The application configuration provider.</param>
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
    /// Creates a JWT containing claims for the specified employee.
    /// </summary>
    /// <param name="employee">The employee to generate a token for.</param>
    /// <returns>A JWT containing the employee's claims.</returns>
    public Jwt CreateJwt(Employee employee)
    {
        return _jwt.Builder.Build([
            new("role", Jwt.Roles.EMPLOYEE),
            new("employee_id", employee.Id.ToString()),
            new("is_verified", employee.IsVerified ? bool.TrueString : bool.FalseString)
        ]);
    }

    /// <summary>
    /// Validates and updates an existing employee in the database.
    /// </summary>
    /// <param name="employee">The employee entity with updated data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateAsync(Employee employee)
    {
        employee.UpdatedAt = DateTimeOffset.UtcNow;

        var (valid, exception) = await _validator.ValidateAsync(employee);
        if (!valid && exception is not null) throw exception;

        _repositories.Employees.Update(employee);
        await _repositories.SaveOrThrowAsync();
    }

    /// <summary>
    /// Sends an email verification link to the specified employee.
    /// </summary>
    /// <param name="employee">The employee to whom the email should be sent.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendEmailVerificationAsync(Employee employee)
    {
        var jwt = CreateJwt(employee);

        // Add a claim to indicate that this token is intended for email verification.
        jwt.Payload.Add(new("purpose", "email_verification"));

        var dto = new EmployeeVerificationEmailDto
        {
            EmployeeName = employee.Name,
            EmployeeAddress = employee.EmailAddress,
            VerificationLink = $"{_configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };

        var message = _email.EmployeeBuilder.BuildVerificationEmail(dto);

        await _email.Sender.SendEmailAsync(message);
    }

    /// <summary>
    /// Confirms the employee's email verification status.
    /// </summary>
    /// <param name="employee">The employee whose email verification is to be confirmed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="BadRequest400Exception">Thrown if the email is already verified.</exception>
    public async Task ConfirmEmailVerificationAsync(Employee employee)
    {
        if (employee.IsVerified)
        {
            throw new BadRequest400Exception("Email is already verified.");
        }

        employee.IsVerified = true;
        await UpdateAsync(employee);
    }
}
