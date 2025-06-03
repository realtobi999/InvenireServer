using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Factories;

/// <summary>
/// Defines a contract for creating mapper instances between entities and DTOs.
/// </summary>
public interface IMapperFactory
{
    /// <summary>
    /// Creates an instance of a mapper for the specified entity and DTO types.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type.</typeparam>
    /// <typeparam name="TDto">The source DTO type.</typeparam>
    /// <returns>An <see cref="IMapper{TEntity, TDto}"/> instance for mapping between the specified types.</returns>
    IMapper<TEntity, TDto> Initiate<TEntity, TDto>();
}
