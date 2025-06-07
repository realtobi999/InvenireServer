using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Application.Factories;

public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _services;

    public ValidatorFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IValidator<TEntity> Initiate<TEntity>()
    {
        var validator = _services.GetService<IValidator<TEntity>>()
                        ?? throw new InvalidOperationException($"Validator for type {typeof(TEntity)} was not found.");

        return validator;
    }
}