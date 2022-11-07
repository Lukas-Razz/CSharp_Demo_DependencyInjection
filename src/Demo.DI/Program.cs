using Autofac;
using Demo.DI;
using Demo.DI.BL.Contracts;
using Demo.DI.Domain;

using var _ioc = new Bootstrapper(Bootstrapper.Provider.Dapper);

using (var scope = _ioc.Container.BeginLifetimeScope())
{
    using var uow = scope.Resolve<IUnitOfWork>();
    var courseRepo = scope.Resolve<ICourseRepository>();
    var enrollRepo = scope.Resolve<IEnrollmentRepository>();

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
    var courses = await courseRepo.GetAllAsync();
    foreach (var c in courses)
    {
        Console.WriteLine(c.Name);
    }
    await uow.CommitAsync();
    var userId = Guid.NewGuid();
    await enrollRepo.CreateAsync(new Enrollment
    {
        Course = courses.First(),
        UserId = userId,
        EnrollmentTimestamp = DateTime.UtcNow,
        ContactEmail = "example@test.example"
    });
    await uow.CommitAsync();
    var enrollments = await enrollRepo.GetAllAsync();
    foreach (var e in enrollments)
    {
        Console.WriteLine($"{e.UserId}::{e.ContactEmail}");
    }
}

using (var scope = _ioc.Container.BeginLifetimeScope())
{
    using var uow2 = scope.Resolve<IUnitOfWork>();
    var repo3 = scope.Resolve<ICourseRepository>();

    await repo3.CreateAsync(new Course
    {
        Name = "New 103",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    await repo3.CreateAsync(new Course
    {
        Name = "New 104",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    foreach (var c in await repo3.GetAllAsync())
    {
        Console.WriteLine(c.Name);
    }

    await uow2.CommitAsync();
}