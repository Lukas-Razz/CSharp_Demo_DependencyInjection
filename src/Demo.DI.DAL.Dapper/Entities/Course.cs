namespace Demo.DI.DAL.Dapper.Entities
{
    public class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public string Location { get; set; }
        public string Contact { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}