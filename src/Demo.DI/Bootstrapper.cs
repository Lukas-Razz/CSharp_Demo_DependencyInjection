using Autofac;
using AutoMapper;
using Demo.DI.DAL.EFCore;
using Demo.DI.Infrastucture.EFCore;
using Microsoft.Data.Sqlite;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DI
{
    public class Bootstrapper : IDisposable
    {
        private SqliteConnection _courseConnection;
        public IContainer Container { get; init; }

        public Bootstrapper()
        {
            // This configures in-memory database
            _courseConnection = new SqliteConnection("Filename=:memory:");
            _courseConnection.Open();

            var builder = new ContainerBuilder();

            builder.RegisterModule(new EFCoreModule(_courseConnection));

            // Registration of AutoMapper profiles
            // Kudos to Martin Loitzl
            // https://blog.loitzl.com/posts/autofac-and-automapper-a-perfect-match
            // Get all Profiles and add them to the MapperConfiguration
            builder.Register(context =>
            {
                var profiles = context.Resolve<IEnumerable<Profile>>();
                var config = new MapperConfiguration(x =>
                {
                    foreach (var profile in profiles)
                    {
                        x.AddProfile(profile);
                    }
                });

                return config;
            }).SingleInstance().AutoActivate().AsSelf();
            // Resolve the MapperConfiguration and call CreateMapper()  
            builder.Register(context =>
            {
                var ctx = context.Resolve<IComponentContext>();
                var config = ctx.Resolve<MapperConfiguration>();
                return config.CreateMapper(
                  t => ctx.Resolve(t) // The magic is happening here: Autofac will now fulfill all dependcies defined in MapperConfigurations
                );
            });

            Container = builder.Build();

            InitDatabaseEFCore();
        }

        private void InitDatabaseEFCore()
        {
            using var context = Container.Resolve<CourseContext>();
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _courseConnection.Dispose();
            Container.Dispose();
        }
    }
}
