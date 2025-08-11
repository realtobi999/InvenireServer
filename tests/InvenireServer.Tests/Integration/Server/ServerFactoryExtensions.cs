using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Tests.Fakers.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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

    public static void ReplaceWithInMemoryDatabase<TContext>(this IServiceCollection services, string name) where TContext : DbContext
    {
        services.RemoveService<IDbContextOptionsConfiguration<TContext>>();
        services.AddDbContext<TContext>(options => { options.UseInMemoryDatabase(name).ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)); });
    }
}