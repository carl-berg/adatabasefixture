using Microsoft.Data.SqlClient;
using Xunit;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Core;

public class DatabaseFixture() : DatabaseFixtureBase(
    new SqlServerDatabaseAdapter(ConnectionFactory, $"ADatabaseMigrator_Tests_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"),
    new FixtureMigrator()), IAsyncLifetime
{
    public static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
}
