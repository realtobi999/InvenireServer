using InvenireServer.Application.Core.Properties.Command.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Extensions.Properties;

public static class PropertyTestExtensions
{
    public static CreatePropertyCommand ToCreatePropertyCommand(this Property property)
    {
        var dto = new CreatePropertyCommand
        {
            Id = property.Id
        };

        return dto;
    }
}
