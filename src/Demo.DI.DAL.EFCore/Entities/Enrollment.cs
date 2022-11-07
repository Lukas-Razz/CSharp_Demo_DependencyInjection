namespace Demo.DI.DAL.EFCore.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid UserId { get; set; }
        public string ContactEmail { get; set; }
        public DateTime EnrollmentTimestamp { get; set; }
        public DateTime? CanceledTimestamp { get; set; }
    }
}