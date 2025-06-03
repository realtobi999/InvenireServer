namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines a contract for validating an entity asynchronously.
/// </summary>
/// <typeparam name="T">The type of the entity to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity instance to validate.</param>
    /// <returns>
    /// A tuple containing a boolean indicating whether the entity is valid,
    /// and an optional exception providing validation failure details.
    /// </returns>
    Task<(bool isValid, Exception? exception)> ValidateAsync(T entity);
}
