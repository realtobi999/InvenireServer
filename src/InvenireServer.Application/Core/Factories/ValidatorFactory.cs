using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Application.Core.Factories;

/// <summary>
/// Factory for resolving validators registered in the service container.
/// </summary>
public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatorFactory"/> class using the given service provider.
    /// </summary>
    /// <param name="services">The service provider used to resolve registered validators.</param>
    public ValidatorFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Resolves a validator instance for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to validate.</typeparam>
    /// <returns>A validator instance for the specified entity type.</returns>
    /// <exception cref="Exception">Thrown if the requested validator is not registered in the service container.</exception>
    public IValidator<TEntity> Initiate<TEntity>()
    {
        var validator = _services.GetService<IValidator<TEntity>>() ?? throw new Exception($"Validator for type {typeof(TEntity)} was not found.");

        return validator;
    }
}
