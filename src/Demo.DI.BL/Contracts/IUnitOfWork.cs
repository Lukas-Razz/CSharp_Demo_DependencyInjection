using System.Data;

namespace Demo.DI.BL.Contracts
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}
