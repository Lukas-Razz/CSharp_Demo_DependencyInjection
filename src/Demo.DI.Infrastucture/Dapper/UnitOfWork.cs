using Demo.DI.DAL.Dapper;
using System.Data;

namespace Demo.DI.Infrastucture.Dapper
{
    public class UnitOfWork : IDapperUnitOfWork
    {
        private CourseContext _context;
        private IDbTransaction _transaction;

        public IDbTransaction Transaction => _transaction;

        public UnitOfWork(CourseContext context)
        {
            _context = context;
            _transaction = context.DbConnection.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            _transaction.Dispose();

            return ValueTask.CompletedTask;
        }

        public Task RollbackAsync()
        {
            _transaction.Rollback();
            _transaction = _context.DbConnection.BeginTransaction();

            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            _transaction.Commit();
            _transaction = _context.DbConnection.BeginTransaction();

            return Task.CompletedTask;
        }
    }
}
