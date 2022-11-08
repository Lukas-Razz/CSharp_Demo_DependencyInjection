using Demo.DI.Domain;
using Optional;

namespace Demo.DI.BL.Services
{
    public interface ICourseService
    {
        Task<Course> EnlistCourse(string name, string location, string contact, DateTime time);
        Task<IEnumerable<Course>> GetAvailableCourses(string location);
        Task<IEnumerable<Course>> GetCoursesBetween(Option<DateTime> from, Option<DateTime> to);
    }
}