using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Extensions.Properties;

/// <summary>
/// Provides test extensions for <see cref="Property"/>.
/// </summary>
public static class PropertyTestExtensions
{
    /// <summary>
    /// Creates a <see cref="CreatePropertyCommand"/> from a property instance.
    /// </summary>
    /// <param name="property">Source property.</param>
    /// <returns>Create property command.</returns>
    public static CreatePropertyCommand ToCreatePropertyCommand(this Property property)
    {
        var dto = new CreatePropertyCommand
        {
            Id = property.Id
        };

        return dto;
    }
}
