using InvenireServer.Application.Interfaces.Common.Transactions;

namespace InvenireServer.Application.Behaviors;

/// <summary>
/// Represents a pipeline behavior that executes before a command is  passed  to
/// its  handler.  This  behavior  is  responsible  for  creating   a   database
/// transaction for commands.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ITransactionScope _transaction;

    public TransactionBehavior(ITransactionScope transaction)
    {
        _transaction = transaction;
    }

    /// <summary>
    /// Handles the request by executing it within a transaction if it is a command.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token)
    {
        // Only apply transaction to commands, not queries.
        if (!typeof(TRequest).Name.EndsWith("Command")) return await next(token);

        return await _transaction.ExecuteAsync(() => next(token));
    }
}