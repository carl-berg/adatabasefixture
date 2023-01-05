using System.Threading.Tasks;
using ADatabaseFixture.FluentMigrator.Tests.Migrations;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase, IAsyncLifetime
    {
        public DatabaseFixture()
            : base(new SqlServerDatabaseAdapter(), FluentMigratorMigrator.Create<CreatePersonTable>(Database.SqlServer2016))
        {
        }
    }
}
