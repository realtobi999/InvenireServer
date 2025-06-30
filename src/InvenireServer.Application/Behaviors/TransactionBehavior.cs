using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IRepositoryManager _repositories;

    public TransactionBehavior(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token)
    {
        // Only apply transaction to commands, not queries.
        if (!typeof(TRequest).Name.EndsWith("Command"))
            return await next(token);

        await using var transaction = await _repositories.BeginTransactionAsync();

        try
        {
            var response = await next(token);
            await transaction.CommitAsync();
            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
