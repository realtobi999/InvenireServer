using System.Linq.Expressions;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Entities.Common;
using Microsoft.AspNetCore.Http;

namespace InvenireServer.Domain.Core.Interfaces.Services;

public interface IEmployeeService
{
    Task<Employee> GetAsync(Jwt jwt);
    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);
    Task CreateAsync(Employee employee);
    Task SendVerificationEmailAsync(Employee employee, HttpRequest request);
    Task AcceptVerificationTokenAsync();
}
