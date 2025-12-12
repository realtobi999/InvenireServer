using System.IO.Compression;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;
using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Commands.GenerateCodes;

public class GenerateCodesPropertyItemsCommandHandler : IRequestHandler<GenerateCodesPropertyItemsCommand, Stream>
{
    private readonly IRepositoryManager _repositories;
    private readonly IQuickResponseCodeGenerator _generator;

    public GenerateCodesPropertyItemsCommandHandler(IQuickResponseCodeGenerator generator, IRepositoryManager repositories)
    {
        _generator = generator;
        _repositories = repositories;
    }

    public const int MAX_LABEL_NAME_LENGTH = 15;

    public async Task<Stream> Handle(GenerateCodesPropertyItemsCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var items = await _repositories.Properties.Items.IndexAsync(new QueryOptions<PropertyItem, PropertyItem>
        {
            Filtering = new QueryFilteringOptions<PropertyItem>
            {
                Filters =
                [
                    i => request.Ids.Contains(i.Id) && i.PropertyId == property.Id,
                ]
            },
        });

        var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            foreach (var item in items.OrderBy(i => i.Location.Building).ThenBy(i => i.Location.Room))
            {
                var labels = new List<string> { item.InventoryNumber, item.Name[..Math.Min(item.Name.Length, MAX_LABEL_NAME_LENGTH)] };
                var code = _generator.GenerateCodeWithLabels(content: $"api/properties/items/{item.Id}/scan", labels: labels, size: request.Size);

                using var entry = archive.CreateEntry($"{item.InventoryNumber.Replace("/", "_")}____{item.Location.Building}_{item.Location.Room}.png", CompressionLevel.Fastest).Open();
                await entry.WriteAsync(code, ct);

                item.LastCodeGeneratedAt = DateTimeOffset.UtcNow;
                _repositories.Properties.Items.Update(item);
            }
            await _repositories.SaveOrThrowAsync();
        }
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static Expression<Func<PropertyItem, PropertyItemDto>> PropertyItemDtoSelector
    {
        get
        {
            return i => new PropertyItemDto
            {
                Id = i.Id,
                InventoryNumber = i.InventoryNumber,
                Name = i.Name,
                Location = new PropertyItemLocationDto
                {
                    Room = i.Location.Room,
                    Building = i.Location.Building,
                },
            };
        }
    }


}
