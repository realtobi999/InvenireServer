using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Factories;

/// <summary>
/// Provides centralized access to factory services used for creating JWTs, mappers, and validators.
/// </summary>
public class FactoryManager : IFactoryManager
{
    private readonly Lazy<IMapperFactory> _mappers;
    private readonly Lazy<IValidatorFactory> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryManager"/> class.
    /// </summary>
    /// <param name="mappers">The factory responsible for creating mappers.</param>
    /// <param name="validators">The factory responsible for creating validators.</param>
    public FactoryManager(IMapperFactory mappers, IValidatorFactory validators)
    {
        _mappers = new Lazy<IMapperFactory>(mappers);
        _validators = new Lazy<IValidatorFactory>(validators);
    }

    /// <inheritdoc/>
    public IMapperFactory Mappers => _mappers.Value;

    /// <inheritdoc/>
    public IValidatorFactory Validators => _validators.Value;
}
