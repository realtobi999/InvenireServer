using System.Linq.Expressions;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<Employee> _validator;

    public EmployeeService(IRepositoryManager repositories, IFactoryManager factories)
    {
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
}
