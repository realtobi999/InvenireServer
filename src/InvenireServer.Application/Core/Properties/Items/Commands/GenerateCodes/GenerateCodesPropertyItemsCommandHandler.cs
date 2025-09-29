using System.IO.Compression;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

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
            foreach (var item in items)
            {
                var code = _generator.GenerateCodeWithLabel(content: $"api/properties/items/{item.Id}/scan", label: item.InventoryNumber);

                var entry = archive.CreateEntry($"{item.InventoryNumber.Replace("/", "_")}.png", CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(code, ct);
            }
        }
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
