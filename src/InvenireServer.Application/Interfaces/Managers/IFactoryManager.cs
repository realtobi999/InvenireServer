using InvenireServer.Application.Interfaces.Factories;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages access to various factory services used throughout the application.
/// </summary>
public interface IFactoryManager
{
    /// <summary>
    /// Gets the factory responsible for creating mapper instances.
    /// </summary>
    IMapperFactory Mappers { get; }

    /// <summary>
    /// Gets the factory responsible for creating validator instances.
    /// </summary>
    IValidatorFactory Validators { get; }
}