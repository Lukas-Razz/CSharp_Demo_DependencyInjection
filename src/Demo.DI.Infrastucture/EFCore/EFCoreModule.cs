using Autofac;
using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Demo.DI.Infrastructure;
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
                .InstancePerLifetimeScope() // This ensures one insance per UnitOfWork
                .OnActivated(e => Console.WriteLine($"Build {e.Instance.GetType().Name}"));
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>();
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
            CreateMap<Domain.Course, DAL.EFCore.Entities.Course>().ReverseMap();

            CreateMap<Domain.Enrollment, DAL.EFCore.Entities.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToNullable()));

            CreateMap<DAL.EFCore.Entities.Enrollment, Domain.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToOption()));
        }
    }
}
