using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace ADatabaseFixture.FluentMigrator
{
    public class FluentMigratorMigrator : IMigrator
    {
        private readonly Action<IMigrationRunnerBuilder> _configure;
        private readonly Assembly[] _assembliesContainingMigrations;

        public static string[] VersioningTables => new[] { "VersionInfo" };

        internal FluentMigratorMigrator(Action<IMigrationRunnerBuilder> configure, params Assembly[] assembliesContainingMigrations)
        {
            _configure = configure;
            _assembliesContainingMigrations = assembliesContainingMigrations;
        }

        public void MigrateUp(string connectionString)
        {
            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => 
                {
                    rb.ScanIn(_assembliesContainingMigrations).For.Migrations();
                    rb.WithGlobalConnectionString(connectionString);
                    _configure(rb);
                })
                .BuildServiceProvider(false);

            using(var scope = serviceProvider.CreateScope())
            {
                scope.ServiceProvider.GetService<IMigrationRunner>().MigrateUp();
            }

        }

        public static IMigrator Create<AssemblyTypeContainingMigration>(Database database) => database switch
        {
            Database.SqlServer2008 => Create<AssemblyTypeContainingMigration>(rb => rb.AddSqlServer2008()),
            Database.SqlServer2012 => Create<AssemblyTypeContainingMigration>(rb => rb.AddSqlServer2012()),
            Database.SqlServer2014 => Create<AssemblyTypeContainingMigration>(rb => rb.AddSqlServer2014()),
            Database.SqlServer2016 => Create<AssemblyTypeContainingMigration>(rb => rb.AddSqlServer2016()),
            _ => throw new ArgumentOutOfRangeException(nameof(database))
        };

        public static IMigrator Create<AssemblyTypeContainingMigration>(Action<IMigrationRunnerBuilder> configure)
        {
            return Create(configure, typeof(AssemblyTypeContainingMigration).Assembly);
        }

        public static IMigrator Create(Action<IMigrationRunnerBuilder> configure, params Assembly[] assembliesContainingMigrations)
        {
            return new FluentMigratorMigrator(configure, assembliesContainingMigrations);
        }
    }
}
