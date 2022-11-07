using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;

namespace Demo.DI.Infrastucture.EFCore
{
    public interface IEFCoreUnitOfWork : IUnitOfWork
    {
        CourseContext Context { get; }
    }
}
