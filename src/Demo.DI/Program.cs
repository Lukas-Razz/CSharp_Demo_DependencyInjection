using Autofac;
using AutoMapper;
using Demo.DI;
using Demo.DI.BL.Contracts;
using Demo.DI.BL.Services;
using Demo.DI.Domain;
using Optional;

using var _ioc = new Bootstrapper(Bootstrapper.Provider.Dapper);

// Scope to control the scoped lifetimes
// If you use some framework (e.g., ASP.NET Core) you get it automatically covered
// But see documentation for exact functionality
using (var scope = _ioc.Container.BeginLifetimeScope())
{
    using var uow = scope.Resolve<IUnitOfWork>();
    var courseRepo = scope.Resolve<ICourseRepository>();
    var enrollRepo = scope.Resolve<IEnrollmentRepository>();

    // Create courses
    await courseRepo.CreateAsync(new Course
    {
        Name = "New 101",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    await courseRepo.CreateAsync(new Course
    {
        Name = "New 102",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    await uow.CommitAsync();

    // List courses
    var courses = await courseRepo.GetAllAsync();
    foreach (var c in courses)
    {
        Console.WriteLine(c.Name);
    }

    // Enroll
    var userId = Guid.NewGuid();
    await enrollRepo.CreateAsync(new Enrollment
    {
        Course = courses.First(),
        UserId = userId,
        EnrollmentTimestamp = DateTime.UtcNow,
        ContactEmail = "example@test.test"
    });
    await uow.CommitAsync();

    // List enrollments
    var enrollments = await enrollRepo.GetAllAsync();
    foreach (var e in enrollments)
    {
        Console.WriteLine($"{e.UserId}::{e.Course.Name}");
    }
}

using (var scope = _ioc.Container.BeginLifetimeScope())
{
    var courseService = scope.Resolve<ICourseService>();
    await courseService.EnlistCourse("New Stuff At Glance", "Ostrava", "example@test.test", DateTime.UtcNow.AddDays(30));
    var courses = await courseService.GetCoursesBetween(DateTime.UtcNow.AddDays(1).Some(), Option.None<DateTime>());
    Console.WriteLine("Upcoming courses:");
    foreach (var c in courses)
    {
        Console.WriteLine(c.Name);
    }
}