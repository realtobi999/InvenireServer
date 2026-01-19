using Serilog;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Defines host builder extension methods.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Configures application configuration sources.
    /// </summary>
    /// <param name="builder">Host builder to configure.</param>
    public static void ConfigureConfiguration(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;

            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment()) config.AddUserSecrets<Program>();

            config.AddEnvironmentVariables();
        });
    }

    /// <summary>
    /// Configures Serilog logging.
    /// </summary>
    /// <param name="builder">Host builder to configure.</param>
    /// <param name="configuration">Configuration containing logging settings.</param>
    public static void ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.UseSerilog();
    }
}
