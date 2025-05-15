using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Application.Core.Mappers;

public class MapperFactory : IMapperFactory
{
    private readonly IServiceProvider _services;

    public MapperFactory(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
    }

    public IMapper<TEntity, TDto> Initiate<TEntity, TDto>()
    {
        var mapper = _services.GetService<IMapper<TEntity, TDto>>() ?? throw new Exception($"Mapper for types {typeof(TEntity)} and {typeof(TDto)} not found.");

        return mapper;
    }
}
