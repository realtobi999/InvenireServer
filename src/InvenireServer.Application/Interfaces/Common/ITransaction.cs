namespace InvenireServer.Application.Interfaces.Common;

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}
