namespace ADatabaseFixture.GalacticWasteManagement.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(), GalacticWasteManagementMigrator.Create<DatabaseFixture>())
        {
        }
    }
}
