using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Application.Core.Factories;

public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _services;

    public ValidatorFactory(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
    }

    public IValidator<TEntity> Initiate<TEntity>()
    {
        var validator = _services.GetService<IValidator<TEntity>>() ?? throw new Exception($"Validator for type {typeof(TEntity)} was not found.");

        return validator;
    }
}
