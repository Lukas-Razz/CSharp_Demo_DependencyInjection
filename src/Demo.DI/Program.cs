using Autofac;
using Demo.DI;
using Demo.DI.BL.Contracts;
using Demo.DI.Domain;
using SimpleInjector.Lifestyles;

using var _ioc = new Bootstrapper();

using (var scope = _ioc.Container.BeginLifetimeScope())
{
    using var uow = scope.Resolve<IUnitOfWork>();
    var repo = scope.Resolve<ICourseRepository>();

    await repo.CreateAsync(new Course
    {
        Name = "New 101",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    await repo.CreateAsync(new Course
    {
        Name = "New 102",
        Location = "Brno",
        Start = DateTime.UtcNow.AddDays(1),
        Contact = "example@example.example"
    });
    foreach (var c in await repo.GetAllAsync())
    {
        Console.WriteLine(c.Name);
    }
    await uow.CommitAsync();
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

//using var _ioc = new Bootstrapper();

//Console.WriteLine("-------");

//using (AsyncScopedLifestyle.BeginScope(_ioc.Container))
//{
//    using var uow = _ioc.Container.GetInstance<IUnitOfWork>();
//    var repo = _ioc.Container.GetInstance<ICourseRepository>();

//    await repo.CreateAsync(new Course 
//    { 
//        Name = "New 101",
//        Location = "Brno",
//        Start = DateTime.UtcNow.AddDays(1),
//        Contact = "example@example.example"
//    });
//    await repo.CreateAsync(new Course
//    {
//        Name = "New 102",
//        Location = "Brno",
//        Start = DateTime.UtcNow.AddDays(1),
//        Contact = "example@example.example"
//    });
//    foreach (var c in await repo.GetAllAsync()) {
//        Console.WriteLine(c.Name);
//    }

//    var repo2 = _ioc.Container.GetInstance<ICourseRepository>();

//    await uow.CommitAsync();
//}

//using (AsyncScopedLifestyle.BeginScope(_ioc.Container))
//{
//    using var uow2 = _ioc.Container.GetInstance<IUnitOfWork>();
//    var repo3 = _ioc.Container.GetInstance<ICourseRepository>();

//    await repo3.CreateAsync(new Course
//    {
//        Name = "New 103",
//        Location = "Brno",
//        Start = DateTime.UtcNow.AddDays(1),
//        Contact = "example@example.example"
//    });
//    await repo3.CreateAsync(new Course
//    {
//        Name = "New 104",
//        Location = "Brno",
//        Start = DateTime.UtcNow.AddDays(1),
//        Contact = "example@example.example"
//    });
//    foreach (var c in await repo3.GetAllAsync())
//    {
//        Console.WriteLine(c.Name);
//    }

//    await uow2.CommitAsync();
//}