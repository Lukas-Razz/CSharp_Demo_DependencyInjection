using Demo.DI.Domain;

namespace Demo.DI.BL.Contracts
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Guid> CreateAsync(Enrollment enrollment);
        Task<Guid> UpdateAsync(Enrollment enrollment);
    }
}
