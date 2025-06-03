namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Provides a contract for mapping from a data transfer object (DTO) to an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to map to.</typeparam>
/// <typeparam name="TDto">The type of the data transfer object to map from.</typeparam>
public interface IMapper<out TEntity, in TDto>
{
    /// <summary>
    /// Maps the specified DTO to an entity instance.
    /// </summary>
    /// <param name="dto">The data transfer object to map.</param>
    /// <returns>The mapped entity instance.</returns>
    TEntity Map(TDto dto);
}

