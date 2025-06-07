namespace InvenireServer.Application.Interfaces.Common;

public interface IValidator<in T>
{
    Task<(bool isValid, Exception? exception)> ValidateAsync(T entity);
}