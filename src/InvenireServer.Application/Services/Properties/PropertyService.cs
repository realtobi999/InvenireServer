using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyService : IPropertyService
{
    public PropertyService()
    {
        Items = new PropertyItemService();
    }

    public IPropertyItemService Items { get; }
}
