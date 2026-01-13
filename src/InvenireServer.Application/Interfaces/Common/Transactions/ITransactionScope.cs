namespace InvenireServer.Application.Interfaces.Common.Transactions;

/// <summary>
/// Defines a database transaction handler.
/// </summary>
public interface ITransactionScope
{
    /// <summary>
    /// Executes an action within a database transaction.
    /// </summary>
    /// <param name="action">Action to execute within the transaction scope.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    Task ExecuteAsync(Func<Task> action);

    /// <summary>
    /// Executes an action within a database transaction and returns its result.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    /// <param name="action">Action to execute within the transaction scope.</param>
    /// <returns>Awaitable task returning the <typeparamref name="T"/> result.</returns>
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
}
