using Demo.DI.DAL.EFCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo.DI.Infrastucture.EFCore
{
    public class UnitOfWork : IEFCoreUnitOfWork
    {
        private CourseContext _context;
        private IDbContextTransaction _transaction;

        public CourseContext Context => _context;

        public UnitOfWork(CourseContext context)
        {
            _context = context;
            _transaction = context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _context.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            _transaction.DisposeAsync();
            _context.DisposeAsync();

            return ValueTask.CompletedTask;
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            _transaction = _context.Database.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
            _transaction = _context.Database.BeginTransaction();
        }
    }
}
