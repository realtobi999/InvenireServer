using InvenireServer.Application.Interfaces.Common.Transactions;

namespace InvenireServer.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ITransactionScope transaction;

    public TransactionBehavior(ITransactionScope transaction)
    {
        this.transaction = transaction;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token)
    {
        // Only apply transaction to commands, not queries.
        if (!typeof(TRequest).Name.EndsWith("Command")) return await next(token);

        return await transaction.ExecuteAsync(() => next(token));
    }
}