using RazorLight;

namespace InvenireServer.Infrastructure.Email.Builders;

/// <summary>
/// Serves as a base class for constructing email messages using Razor HTML templates.
/// </summary>
public abstract class BaseEmailBuilder
{
    /// <summary>
    /// The email address from which the message is sent.
    /// </summary>
    protected readonly string SourceAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEmailBuilder"/> class.
    /// </summary>
    /// <param name="source">The sender's email address.</param>
    protected BaseEmailBuilder(string source)
    {
        SourceAddress = source;
    }

    /// <summary>
    /// Loads and parses a Razor-based HTML template with the given model.
    /// </summary>
    /// <param name="filepath">The file path of the HTML template to load.</param>
    /// <param name="model">The data model to inject into the template.</param>
    /// <returns>The resulting HTML string with model data rendered.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified template file does not exist.</exception>
    protected string ParseHtmlTemplate(string filepath, object model)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("The specified HTML template file was not found.", filepath);
        }

        var html = File.ReadAllText(filepath);

        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.GetDirectoryName(filepath))
            .UseMemoryCachingProvider()
            .Build();

        var body = engine.CompileRenderStringAsync(Guid.NewGuid().ToString(), html, model).Result;

        return body;
    }
}
