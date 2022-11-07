using Autofac;
using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.Dapper;
using Entities = Demo.DI.DAL.Dapper.Entities;
using Microsoft.Data.Sqlite;
using Optional.Unsafe;
using Optional;

namespace Demo.DI.Infrastucture.Dapper
{
    public class DapperModule : Module
    {
        private SqliteConnection _connection;
        public DapperModule(SqliteConnection connection)
        {
            _connection = connection;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<CourseContext>(ctx => new CourseContext(_connection))
                .InstancePerDependency()
                .OnActivated(e => Console.WriteLine($"Build {e.Instance.GetType().Name}"));
            builder.RegisterType<UnitOfWork>()
                .InstancePerLifetimeScope() // This ensures only one UoW per Repos, as SQLite does not support nested transactions
                .As<IUnitOfWork>()
                .As<IDapperUnitOfWork>() // This registres UnitOfWork for both interfaces. One "internal" and one "external"
                .OnActivated(e => Console.WriteLine($"Build {e.Instance.GetType().Name}"));
            builder.RegisterType<CourseRepository>()
                .As<ICourseRepository>();
            builder.RegisterType<EnrollmentRepository>()
                .As<IEnrollmentRepository>();

            // AutoMapper Profile
            builder.RegisterType<DapperProfile>()
                .As<Profile>()
                .AutoActivate();
        }
    }

    // Configuration for AutoMapper
    internal class DapperProfile : Profile
    {
        public DapperProfile()
        {
            CreateMap<Domain.Course, Entities.Course>();

            CreateMap<Entities.Course, Domain.Course>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<Domain.Enrollment, Entities.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToNullable()));

            CreateMap<Entities.Enrollment, Domain.Enrollment>()
                .ForMember(d => d.CanceledTimestamp, m => m.MapFrom(s => s.CanceledTimestamp.ToOption()))
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.UserId, m => m.MapFrom(s => Guid.Parse(s.UserId)));
        }
    }
}
