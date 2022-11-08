using Autofac;
using Demo.DI.BL.Contracts;
using Demo.DI.BL.Services;
using Demo.DI.Infrastructure;
using Demo.DI.Infrastucture.Dapper;
using Demo.DI.Infrastucture.EFCore;
using Microsoft.Data.Sqlite;
using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using System.Net.Mail;

namespace Demo.DI
{
    public class Bootstrapper : IDisposable
    {
        private SqliteConnection _courseConnection;
        public IContainer Container { get; init; }

        public Bootstrapper(Provider provider)
        {
            // This configures in-memory database
            _courseConnection = new SqliteConnection("Filename=:memory:");
            _courseConnection.Open();

            var builder = new ContainerBuilder();

            // Coose ORM
            Module module = provider switch
            {
                Provider.EFCore => new EFCoreModule(_courseConnection),
                Provider.Dapper => new DapperModule(_courseConnection),
            };
            builder.RegisterModule(module);

            // Register BL Services
            builder.RegisterType<CourseService>()
                .As<ICourseService>();
            builder.RegisterType<EnrollmentService>()
                .As<IEnrollmentService>();

            // Email
            builder.RegisterType<EmailService>()
                .As<IEmailService>();
            var smtpClient = new SmtpClient("smtp-relay.sendinblue.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("", "")
            };
            builder.Register(ctx => new SmtpSender(smtpClient))
                .As<ISender>();

            // See BootstraperExtensions
            builder.RegisterAutoMapper();

            Container = builder.Build();

            // Init database
            Action dbInit = provider switch
            {
                Provider.EFCore => InitDatabaseEFCore,
                Provider.Dapper => InitDatabaseDapper,
            };
            dbInit();
        }

        private void InitDatabaseEFCore()
        {
            using var context = Container.Resolve<DAL.EFCore.CourseContext>();
            context.Database.EnsureCreated();
        }

        private void InitDatabaseDapper()
        {
            var context = Container.Resolve<DAL.Dapper.CourseContext>();
            context.CreateDatabase().GetAwaiter().GetResult(); // Can't await in constructor
        }

        public void Dispose()
        {
            _courseConnection.Dispose();
            Container.Dispose();
        }

        public enum Provider
        {
            EFCore,
            Dapper
        }
    }
}
