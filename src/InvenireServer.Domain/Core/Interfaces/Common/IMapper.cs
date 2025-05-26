namespace InvenireServer.Domain.Core.Interfaces.Common;

public interface IMapper<out TEntity, in TDto>
{
    TEntity Map(TDto dto);
}
