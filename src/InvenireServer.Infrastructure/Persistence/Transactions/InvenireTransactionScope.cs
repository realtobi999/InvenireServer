using InvenireServer.Application.Interfaces.Common.Transactions;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Transactions;

public class InvenireTransactionScope : ITransactionScope
{
    private readonly InvenireServerContext _context;

    public InvenireTransactionScope(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var result = await action();

            await transaction.CommitAsync();

            return result;
        });
    }
}