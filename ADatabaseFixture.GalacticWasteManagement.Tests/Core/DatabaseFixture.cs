using System;
using Xunit;

namespace ADatabaseFixture.GalacticWasteManagement.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase, IAsyncLifetime
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(databaseName: DatabaseName()), GalacticWasteManagementMigrator.Create<DatabaseFixture>())
        {
        }

        private static string DatabaseName() => $"GWM_TestDatabase_{DateTime.Now:yyyy-MM-dd_HH-mm}";
    }
}
