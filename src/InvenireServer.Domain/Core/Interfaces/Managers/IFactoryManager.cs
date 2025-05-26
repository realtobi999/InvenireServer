using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Domain.Core.Interfaces.Managers;

public interface IFactoryManager
{
    IJwtFactory Jwt { get; }
    IMapperFactory Mappers { get; }
    IValidatorFactory Validators { get; }
}
