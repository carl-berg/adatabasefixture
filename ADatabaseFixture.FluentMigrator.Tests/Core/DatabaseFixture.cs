using ADatabaseFixture.FluentMigrator.Tests.Migrations;

namespace ADatabaseFixture.FluentMigrator.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(), FluentMigratorMigrator.Create<CreatePersonTable>(Database.SqlServer2016))
        {
        }
    }
}
