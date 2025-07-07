using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Factories;

public interface IEntityValidatorFactory
{
    IEntityValidator<TEntity> Initiate<TEntity>();
}