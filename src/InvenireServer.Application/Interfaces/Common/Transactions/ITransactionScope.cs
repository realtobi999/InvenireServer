namespace InvenireServer.Application.Interfaces.Common.Transactions;

public interface ITransactionScope
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    Task ExecuteAsync(Func<Task> action);
}