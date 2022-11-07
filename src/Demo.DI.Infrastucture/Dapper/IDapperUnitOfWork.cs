using Demo.DI.BL.Contracts;
using System.Data;

namespace Demo.DI.Infrastucture.Dapper
{
    public interface IDapperUnitOfWork : IUnitOfWork
    {
        IDbTransaction Transaction { get; }
    }
}
