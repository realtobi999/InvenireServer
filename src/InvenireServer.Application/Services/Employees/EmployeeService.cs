using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Employees;

namespace InvenireServer.Application.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<Employee> _validator;

    public EmployeeService(IRepositoryManager repositories, IValidator<Employee> validator)
    {
        _validator = validator;
        _repositories = repositories;
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

    public async Task UpdateAsync(Employee employee)
    {
        await UpdateAsync([employee]);
    }

    public async Task UpdateAsync(IEnumerable<Employee> employees)
    {
        foreach (var employee in employees)
        {
            employee.LastUpdatedAt = DateTimeOffset.UtcNow;

            var (valid, exception) = await _validator.ValidateAsync(employee);
            if (!valid && exception is not null) throw exception;

            _repositories.Employees.Update(employee);
        }

        await _repositories.SaveOrThrowAsync();
    }
}