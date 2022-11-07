using Demo.DI.Domain;

namespace Demo.DI.BL.Contracts
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Guid> CreateAsync(Course course);
    }
}
