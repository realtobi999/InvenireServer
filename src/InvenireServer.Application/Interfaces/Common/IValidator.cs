namespace InvenireServer.Application.Interfaces.Common;

public interface IEntityValidator<in T>
{
    Task<(bool isValid, Exception? exception)> ValidateAsync(T entity);
}