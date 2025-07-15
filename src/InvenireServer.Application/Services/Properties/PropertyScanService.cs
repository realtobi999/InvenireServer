using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Properties;
using FluentValidation.Results;
using InvenireServer.Domain.Validators.Properties;
using FluentValidation;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Services.Properties;

public class PropertyScanService : IPropertyScanService
{
    private readonly IRepositoryManager _repositories;

    public PropertyScanService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public Task<IEnumerable<PropertyScan>> IndexActiveAsync()
    {
        return _repositories.Properties.Scans.IndexActiveAsync();
    }

    public async Task<PropertyScan> GetAsync(Expression<Func<PropertyScan, bool>> predicate)
    {
        var scan = await TryGetAsync(predicate);

        if (scan is null) throw new NotFound404Exception($"The requested {nameof(PropertyScan).ToLower()} was not found in the system");

        return scan;
    }

    public async Task<PropertyScan?> TryGetAsync(Expression<Func<PropertyScan, bool>> predicate)
    {
        var scan = await _repositories.Properties.Scans.GetAsync(predicate);

        return scan;
    }

    public async Task CreateAsync(PropertyScan scan)
    {
        var result = new ValidationResult(PropertyScanEntityValidator.Validate(scan));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(PropertyScan).ToLower()} (ID: {scan.Id}).", result.Errors);

        _repositories.Properties.Scans.Create(scan);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(PropertyScan scan)
    {
        var result = new ValidationResult(PropertyScanEntityValidator.Validate(scan));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(PropertyScan).ToLower()} (ID: {scan.Id}).", result.Errors);

        _repositories.Properties.Scans.Update(scan);
        await _repositories.SaveOrThrowAsync();
    }
}
