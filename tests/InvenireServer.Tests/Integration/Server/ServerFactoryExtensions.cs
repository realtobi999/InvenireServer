using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Tests.Fakers.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Integration.Server;

/// <summary>
/// Defines service collection helpers for test server setup.
/// </summary>
public static class ServerFactoryExtensions
{
    /// <summary>
    /// Removes the first registration matching the service type.
    /// </summary>
    /// <typeparam name="TService">Service type to remove.</typeparam>
    /// <param name="services">Service collection to modify.</param>
    public static void RemoveService<TService>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null) services.Remove(descriptor);
    }

    /// <summary>
    /// Replaces the email sender with a fake implementation.
    /// </summary>
    /// <param name="services">Service collection to modify.</param>
    public static void ReplaceWithFakeEmailSender(this IServiceCollection services)
    {
        services.RemoveService<IEmailSender>();
        services.AddSingleton<IEmailSender, EmailSenderFaker>();
    }

    /// <summary>
    /// Replaces the DbContext configuration with an in-memory database.
    /// </summary>
    /// <typeparam name="TContext">DbContext type.</typeparam>
    /// <param name="services">Service collection to modify.</param>
    /// <param name="name">In-memory database name.</param>
    public static void ReplaceWithInMemoryDatabase<TContext>(this IServiceCollection services, string name) where TContext : DbContext
    {
        services.RemoveService<IDbContextOptionsConfiguration<TContext>>();
        services.AddDbContext<TContext>(options => { options.UseInMemoryDatabase(name).ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)); });
    }
}
