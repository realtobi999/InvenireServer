namespace InvenireServer.Domain.Core.Interfaces.Common;

public interface IMapper<TEntity, TDto>
{
    TEntity Map(TDto dto);
}
