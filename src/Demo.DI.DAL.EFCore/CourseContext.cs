using Demo.DI.DAL.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.DI.DAL.EFCore
{
    public class CourseContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public CourseContext(DbContextOptions<CourseContext> options) : base(options)
        {
        }
    }
}