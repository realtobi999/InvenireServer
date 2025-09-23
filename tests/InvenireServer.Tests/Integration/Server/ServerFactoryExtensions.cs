using SQLitePCL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Tests.Fakers.Common;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Application.Interfaces.Email;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InvenireServer.Tests.Integration.Server;

public static class ServerFactoryExtensions
{
    public static void RemoveService<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null) services.Remove(descriptor);
    }

    public static void ReplaceWithFakeEmailSender(this IServiceCollection services)
    {
        services.RemoveService<IEmailSender>();
        services.AddSingleton<IEmailSender, EmailSenderFaker>();
    }

    public static void ReplaceWithInMemoryDatabase<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.RemoveService<IDbContextOptionsConfiguration<TContext>>();

        Batteries.Init();

        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlite(connection);
        });

        // Build a temporary provider to run the migrations.
        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        db.Database.Migrate();
    }
}