using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Application.Validators;

public class EntityValidatorFactory : IEntityValidatorFactory
{
    private readonly IServiceProvider _services;

    public EntityValidatorFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IEntityValidator<TEntity> Initiate<TEntity>()
    {
        var validator = _services.GetService<IEntityValidator<TEntity>>() ?? throw new InvalidOperationException($"Validator for type {typeof(TEntity)} was not found.");

        return validator;
    }
}