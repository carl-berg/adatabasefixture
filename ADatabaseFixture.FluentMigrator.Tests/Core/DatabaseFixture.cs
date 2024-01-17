using System;
using ADatabaseFixture.FluentMigrator.Tests.Migrations;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase, IAsyncLifetime
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(databaseName: DatabaseName()), FluentMigratorMigrator.Create<CreatePersonTable>(Database.SqlServer2016))
        {
        }

        private static string DatabaseName() => $"FluentMigrator_TestDatabase_{DateTime.Now:yyyy-MM-dd_HH-mm}";
    }
}
