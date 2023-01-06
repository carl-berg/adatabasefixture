using Xunit;

namespace ADatabaseFixture.GalacticWasteManagement.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase, IAsyncLifetime
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(), GalacticWasteManagementMigrator.Create<DatabaseFixture>())
        {
        }
    }
}
