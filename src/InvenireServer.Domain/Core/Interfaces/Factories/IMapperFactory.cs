using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Interfaces.Factories;

public interface IMapperFactory
{
    IMapper<TEntity, TDto> Initiate<TEntity, TDto>();
}
