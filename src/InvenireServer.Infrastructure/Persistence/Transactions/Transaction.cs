using InvenireServer.Application.Interfaces.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace InvenireServer.Infrastructure.Persistence.Transactions;

public class Transaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public Transaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync()
    {
        return _transaction.CommitAsync();
    }

    public Task RollbackAsync()
    {
        return _transaction.RollbackAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
