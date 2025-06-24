using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Factories;

public interface IValidatorFactory
{
    IValidator<TEntity> Initiate<TEntity>();
}
