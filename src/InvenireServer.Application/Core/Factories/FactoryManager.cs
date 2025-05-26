using InvenireServer.Domain.Core.Interfaces.Factories;
using InvenireServer.Domain.Core.Interfaces.Managers;

namespace InvenireServer.Application.Core.Factories;

public class FactoryManager : IFactoryManager
{
    private readonly Lazy<IJwtFactory> _jwt;
    private readonly Lazy<IMapperFactory> _mappers;
    private readonly Lazy<IValidatorFactory> _validators;

    public FactoryManager(IMapperFactory mappers, IValidatorFactory validators, IJwtFactory jwt)
    {
        _jwt = new Lazy<IJwtFactory>(jwt);
        _mappers = new Lazy<IMapperFactory>(mappers);
        _validators = new Lazy<IValidatorFactory>(validators);
    }

    public IJwtFactory Jwt => _jwt.Value;
    public IMapperFactory Mappers => _mappers.Value;
    public IValidatorFactory Validators => _validators.Value;
}
