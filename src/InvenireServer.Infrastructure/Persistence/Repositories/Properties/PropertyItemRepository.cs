using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
    }

    public Expression<Func<PropertyItem, bool>> BuildSearchExpression(string term)
    {
        if (term.StartsWith('\"') && term.EndsWith('\"'))
        {
            term = term.Trim('"');
        }
        else
        {
            term = $"%{term}%";
        }

        return i =>
            EF.Functions.ILike(i.Name, term) ||
            EF.Functions.ILike(i.InventoryNumber, term) ||
            EF.Functions.ILike(i.RegistrationNumber, term);
    }

    public override void Update(PropertyItem item)
    {
        item.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(item);
    }
}
