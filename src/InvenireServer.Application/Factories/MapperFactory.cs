using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Application.Factories;

/// <summary>
/// Factory for resolving mappers registered in the service container.
/// </summary>
public class MapperFactory : IMapperFactory
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapperFactory"/> class using the given service provider.
    /// </summary>
    /// <param name="services">The service provider used to resolve registered mappers.</param>
    public MapperFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Resolves a mapper instance for the specified entity and DTO types.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDto">The type of the DTO.</typeparam>
    /// <returns>A mapper instance for the given entity and DTO types.</returns>
    /// <exception cref="Exception">Thrown if the requested mapper is not registered in the service container.</exception>
    public IMapper<TEntity, TDto> Initiate<TEntity, TDto>()
    {
        var mapper = _services.GetService<IMapper<TEntity, TDto>>() ?? throw new Exception($"Mapper for types {typeof(TEntity)} and {typeof(TDto)} was not found.");

        return mapper;
    }
}
