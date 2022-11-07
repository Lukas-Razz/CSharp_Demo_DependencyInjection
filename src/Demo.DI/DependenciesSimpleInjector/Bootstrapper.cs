using Demo.DI.DAL.EFCore;
using Demo.DI.Infrastucture.EFCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Demo.DI.DependenciesSimpleInjector
{
    public class Bootstrapper : IDisposable
    {
        private SqliteConnection _courseConnection;

        public Container Container { get; init; }

        public Bootstrapper()
        {
            // This configures in-memory database
            _courseConnection = new SqliteConnection("Filename=:memory:");
            _courseConnection.Open();

            // Create a new Simple Injector container
            var container = new Container();

            // Configure the container (register)
            //
            // Injects a "module" see https://docs.simpleinjector.org/en/latest/howto.html#package-registrations
            // Packages are good if you are working whith modular appliaction, for that case you can use automatic assemby scanning.
            // Normaly you want have configuration as close to the "Composition Root" as in this case the Console.
            container.BootstrapEFCore(_courseConnection);

            // Verify your configuration
            container.Verify();

            // Initialise the Database
            container.InitDatabaseEFCore();

            Container = container;
        }

        public void Dispose()
        {
            _courseConnection.Dispose();
            Container.Dispose();
        }
    }
}
