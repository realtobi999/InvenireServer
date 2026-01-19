using RazorLight;

namespace InvenireServer.Infrastructure.Email.Builders;

/// <summary>
/// Base class for building emails from HTML templates.
/// </summary>
public abstract class BaseEmailBuilder
{
    protected readonly string SourceAddress;

    protected BaseEmailBuilder(string source)
    {
        SourceAddress = source;
    }

    /// <summary>
    /// Parses an HTML template with the provided model.
    /// </summary>
    /// <param name="filepath">Template file path.</param>
    /// <param name="model">Model used to render the template.</param>
    /// <returns>Rendered HTML content.</returns>
    protected string ParseHtmlTemplate(string filepath, object model)
    {
        if (!File.Exists(filepath)) throw new FileNotFoundException("The specified HTML template file was not found.", filepath);

        var html = File.ReadAllText(filepath);

        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.GetDirectoryName(filepath))
            .UseMemoryCachingProvider()
            .Build();

        var body = engine.CompileRenderStringAsync(Guid.NewGuid().ToString(), html, model).Result;

        return body;
    }
}
