namespace Demo.DI.DAL.Dapper.Entities
{
    public class Enrollment
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public string UserId { get; set; }
        public string ContactEmail { get; set; }
        public DateTime EnrollmentTimestamp { get; set; }
        public DateTime? CanceledTimestamp { get; set; }
    }
}