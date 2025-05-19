using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Domain.Core.Interfaces.Factories;

public interface IValidatorFactory
{
    IValidator<TEntity> Initiate<TEntity>();
}
