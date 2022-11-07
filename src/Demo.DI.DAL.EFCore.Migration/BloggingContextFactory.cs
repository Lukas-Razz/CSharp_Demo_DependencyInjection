using Demo.DI.DAL.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class BloggingContextFactory : IDesignTimeDbContextFactory<CourseContext>
{
    public CourseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseContext>();
        optionsBuilder.UseSqlite("Data Source=temp.db", b => b.MigrationsAssembly("Demo.DI.DAL.EFCore.Migration"));

        return new CourseContext(optionsBuilder.Options);
    }
}