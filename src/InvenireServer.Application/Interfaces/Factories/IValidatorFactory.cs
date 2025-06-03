using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Factories;

/// <summary>
/// Defines a contract for creating validator instances for specified entity types.
/// </summary>
public interface IValidatorFactory
{
    /// <summary>
    /// Creates an instance of a validator for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to validate.</typeparam>
    /// <returns>An <see cref="IValidator{TEntity}"/> instance for validating the specified entity type.</returns>
    IValidator<TEntity> Initiate<TEntity>();
}
