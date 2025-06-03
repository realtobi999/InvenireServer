using Serilog;

namespace InvenireServer.Presentation.Extensions;

/// <summary>
/// Provides extension methods for configuring the host builder.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Configures the application configuration sources, including JSON files, user secrets in development, and environment variables.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    public static void ConfigureConfiguration(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;

            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                config.AddUserSecrets<Program>();
            }

            config.AddEnvironmentVariables();
        });
    }

    /// <summary>
    /// Configures Serilog as the logging provider using the provided configuration.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="configuration">The application configuration containing Serilog settings.</param>
    public static void ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.UseSerilog();
    }
}