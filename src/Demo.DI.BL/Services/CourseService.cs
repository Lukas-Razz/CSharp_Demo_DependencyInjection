using Ardalis.GuardClauses;
using Demo.DI.BL.Contracts;
using Demo.DI.Domain;
using Optional;

namespace Demo.DI.BL.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Course> EnlistCourse(string name, string location, string contact, DateTime time)
        {
            Guard.Against.NullOrWhiteSpace(name);
            Guard.Against.NullOrWhiteSpace(location);
            // A simple email regex, it does not cover all valid emails
            Guard.Against.InvalidFormat(contact, nameof(contact), @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

            var newCourse = new Course
            {
                Name = name,
                Location = location,
                Contact = contact,
                Start = time,
            };

            var newCourseId = await _courseRepository.CreateAsync(newCourse);

            return await _courseRepository.GetAsync(newCourseId);
        }

        // Retruns all courses at given location
        // Parameter must be non-empty string
        public async Task<IEnumerable<Course>> GetAvailableCourses(string location)
        {
            Guard.Against.NullOrWhiteSpace(location);

            var courses = await _courseRepository.GetAllAsync();

            return courses.Where(c => c.Location == location && c.Start > DateTime.UtcNow);
        }

        // Retruns all courses in the interval
        // Interval can be open-ended
        // Both parameters cannot be None at the same time
        public async Task<IEnumerable<Course>> GetCoursesBetween(Option<DateTime> from, Option<DateTime> to)
        {
            Guard.Against.AgainstExpression(a => a.from.HasValue || a.to.HasValue, (from, to), "At least one argument must be spefified");

            var courses = await _courseRepository.GetAllAsync();

            from.MatchSome(f => { courses = courses.Where(c => c.Start >= f); });
            to.MatchSome(t => { courses = courses.Where(c => c.Start <= t); });

            return courses;
        }
    }
}
