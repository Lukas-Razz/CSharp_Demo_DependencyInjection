using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo.DI.Infrastucture.EFCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private CourseContext _context;
        private IDbContextTransaction _transaction;

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

        public Task RollbackAsync()
        {
            return _transaction.RollbackAsync();
        }

        public Task CommitAsync()
        {
            return _transaction.CommitAsync();
        }
    }
}
