using Microsoft.Data.SqlClient;
using Xunit;

namespace ADatabaseFixture.EfCoreMigration.Tests.Core;

public class DatabaseFixture() : DatabaseFixtureBase(
    new SqlServerDatabaseAdapter(ConnectionFactory, $"EFCoreMigrator_Tests_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"),
    new FixtureMigrator()), IAsyncLifetime
{
    public static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
}
