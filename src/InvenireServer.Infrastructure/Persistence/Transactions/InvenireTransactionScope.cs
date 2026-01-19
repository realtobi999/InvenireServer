using InvenireServer.Application.Interfaces.Common.Transactions;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Transactions;

/// <summary>
/// Default implementation of <see cref="ITransactionScope"/>.
/// </summary>
public class InvenireTransactionScope : ITransactionScope
{
    private readonly InvenireServerContext _context;

    public InvenireTransactionScope(InvenireServerContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Executes an action within a database transaction and returns its result.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="action">Action to execute within the transaction scope.</param>
    /// <returns>Awaitable task returning the <typeparamref name="T"/> result.</returns>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            if (_context.Database.CurrentTransaction is not null) return await action();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var result = await action();
            await transaction.CommitAsync();

            return result;
        });
    }

    /// <summary>
    /// Executes an action within a database transaction.
    /// </summary>
    /// <param name="action">Action to execute within the transaction scope.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task ExecuteAsync(Func<Task> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            if (_context.Database.CurrentTransaction is not null) await action();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            await action();
            await transaction.CommitAsync();
        });
    }
}
