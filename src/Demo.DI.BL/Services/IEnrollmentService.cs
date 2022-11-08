using Demo.DI.Domain;

namespace Demo.DI.BL.Services
{
    public interface IEnrollmentService
    {
        Task CancelEnrollment(string courseName, Guid userId);
        Task EnrollCourse(string courseName, Guid userId, string contact);
        Task<IEnumerable<Enrollment>> GetEnrollmentsFor(Guid userId);
    }
}