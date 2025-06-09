using Serilog;

namespace InvenireServer.Presentation.Extensions;

public static class HostExtensions
{
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

    public static void ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.UseSerilog();
    }
}