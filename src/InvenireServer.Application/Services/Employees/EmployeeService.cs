using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Services.Employees;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Validators.Users;

namespace InvenireServer.Application.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repositories;

    public EmployeeService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Dto = new EmployeeDtoService(repositories);
    }

    public IEmployeeDtoService Dto { get; }

    public Task<IEnumerable<Employee>> IndexInactiveAsync()
    {
        return _repositories.Employees.IndexInactiveAsync();
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
        var result = new ValidationResult(EmployeeEntityValidator.Validate(employee));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Employee).ToLower()} (ID: {employee.Id}).", result.Errors);

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

            var result = new ValidationResult(EmployeeEntityValidator.Validate(employee));
            if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Employee).ToLower()} (ID: {employee.Id}).", result.Errors);

            _repositories.Employees.Update(employee);
        }

        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        await DeleteAsync([employee]);
    }

    public async Task DeleteAsync(IEnumerable<Employee> employees)
    {
        foreach (var employee in employees) _repositories.Employees.Delete(employee);
        await _repositories.SaveOrThrowAsync();
    }
}

public class EmployeeDtoService : IEmployeeDtoService
{
    private readonly IRepositoryManager _repositories;

    public EmployeeDtoService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<EmployeeDto> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    public async Task<EmployeeDto> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        var employeeDto = await _repositories.Employees.Dto.GetAsync(predicate);

        if (employeeDto is null) throw new NotFound404Exception($"The requested {nameof(Employee).ToLower()} was not found in the system.");

        return employeeDto;
    }
}