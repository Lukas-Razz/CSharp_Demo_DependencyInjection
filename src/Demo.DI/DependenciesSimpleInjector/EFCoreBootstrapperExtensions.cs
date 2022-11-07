using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Demo.DI.Infrastructure;
using Demo.DI.Infrastucture.EFCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Unsafe;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Demo.DI.DependenciesSimpleInjector
{
    // This just packages the registrations for EFCore
    public static class EFCoreBootstrapperExtensions
    {
        public static void BootstrapEFCore(this Container container, SqliteConnection connection)
        {
            var courseContextObtions = new DbContextOptionsBuilder<CourseContext>()
                .UseSqlite(connection)
                .Options;

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;

            container.Register<CourseContext>(() => new CourseContext(courseContextObtions));
            container.Register<IUnitOfWork, UnitOfWork>();
            container.Register<ICourseRepository, CourseRepository>();
            container.Register<IEnrollmentRepository, EnrollmentRepository>();

            // This is required to integrate with SimpleInjector
            // Other DIs handle it differently
            // https://docs.automapper.org/en/v11.0.0/Dependency-injection.html
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(container.GetInstance);
                cfg.AddProfile(new EFCoreProfile());
            });

            container.Register<IMapper>(mapperConfig.CreateMapper);
        }

        public static void InitDatabaseEFCore(this Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                using var context = container.GetInstance<CourseContext>();
                context.Database.EnsureCreated();
            }
        }
    }

    // Configuration for AutoMapper
    internal class EFCoreProfile : Profile
    {
        public EFCoreProfile()
        {
            CreateMap<Domain.Course, DAL.EFCore.Entities.Course>().ReverseMap();

            CreateMap<Domain.Enrollment, DAL.EFCore.Entities.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToNullable()));

            CreateMap<DAL.EFCore.Entities.Enrollment, Domain.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToOption()));
        }
    }

}