using Ardalis.GuardClauses;
using Demo.DI.BL.Contracts;
using Demo.DI.Domain;

namespace Demo.DI.BL.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEmailService _emailService;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository, IEmailService emailService)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _emailService = emailService;
        }

        // Get all enrollments given some user id
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsFor(Guid userId)
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();

            return enrollments.Where(e => e.UserId == userId);
        }

        // Create new course
        // Throws exception if the specified course is not found
        // Send email if successful
        // String parameters must be non-empty strings
        public async Task EnrollCourse(string courseName, Guid userId, string contact)
        {
            Guard.Against.NullOrWhiteSpace(courseName);
            Guard.Against.NullOrWhiteSpace(contact);

            var courseToEnroll = await _courseRepository.GetByNameAsync(courseName);

            if (courseToEnroll is null)
                throw new InvalidOperationException("Specified course does not exist");

            var enrollment = new Enrollment
            {
                Course = courseToEnroll,
                ContactEmail = contact,
                EnrollmentTimestamp = DateTime.UtcNow,
                UserId = userId,
            };

            await _enrollmentRepository.CreateAsync(enrollment);

            await _emailService.SendEmailAsync(new Email(contact, "Course Enrolled", "Yes"));
        }

        // Canceles existing enrollment to a course
        // Throws exception if the enrollment is not found
        // Send email if successful
        // String parameters must be non-empty strings
        public async Task CancelEnrollment(string courseName, Guid userId)
        {
            Guard.Against.NullOrWhiteSpace(courseName);

            var courseToEnroll = await _courseRepository.GetByNameAsync(courseName);

            if (courseToEnroll is null)
                throw new InvalidOperationException("Specified course does not exist");

            var enrollments = await _enrollmentRepository.GetAllAsync();

            var enrollmentToCancel = enrollments
                .FirstOrDefault(e => e.UserId == userId && e.Course.Id == courseToEnroll.Id);

            if (enrollmentToCancel is null)
                new InvalidOperationException("Specified enrollment does not exist");

            await _enrollmentRepository.UpdateAsync(enrollmentToCancel);

            await _emailService.SendEmailAsync(new Email(enrollmentToCancel?.ContactEmail, "Course Enrolled", "Yes"));

        }

    }
}
