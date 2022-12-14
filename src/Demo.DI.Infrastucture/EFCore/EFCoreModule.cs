using Autofac;
using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Entities = Demo.DI.DAL.EFCore.Entities;
using Demo.DI.Infrastructure.EFCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Unsafe;

namespace Demo.DI.Infrastucture.EFCore
{
    public class EFCoreModule : Module
    {
        private SqliteConnection _connection;
        public EFCoreModule(SqliteConnection connection)
        {
            _connection = connection;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var courseContextObtions = new DbContextOptionsBuilder<CourseContext>()
                .UseSqlite(_connection)
                .Options;

            builder.Register<CourseContext>(ctx => new CourseContext(courseContextObtions))
                .InstancePerDependency()
                .OnActivated(e => Console.WriteLine($"Build {e.Instance.GetType().Name}"));
            builder.RegisterType<UnitOfWork>()
                .InstancePerLifetimeScope() // This ensures only one UoW per Repos, as SQLite does not support nested transactions
                .As<IUnitOfWork>()
                .As<IEFCoreUnitOfWork>() // This registres UnitOfWork for both interfaces. One "internal" and one "external"
                .OnActivated(e => Console.WriteLine($"Build {e.Instance.GetType().Name}"));
            builder.RegisterType<CourseRepository>()
                .As<ICourseRepository>();
            builder.RegisterType<EnrollmentRepository>()
                .As<IEnrollmentRepository>();

            // AutoMapper Profile
            builder.RegisterType<EFCoreProfile>()
                .As<Profile>()
                .AutoActivate();
        }
    }

    // Configuration for AutoMapper
    internal class EFCoreProfile : Profile
    {
        public EFCoreProfile()
        {
            CreateMap<Domain.Course, Entities.Course>().ReverseMap();

            CreateMap<Domain.Enrollment, Entities.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToNullable()))
                .ForMember(d => d.CourseId, m => m.MapFrom(s => s.Course.Id))
                .ForMember(d => d.Course, m => m.Ignore());

            CreateMap<Entities.Enrollment, Domain.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToOption()));
        }
    }
}
