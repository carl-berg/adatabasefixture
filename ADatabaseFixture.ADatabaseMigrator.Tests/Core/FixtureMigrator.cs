using ADatabaseFixture.ADatabaseMigrator.Tests.Database;
using Microsoft.Data.SqlClient;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Core;

public class FixtureMigrator : IMigrator
{
    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken = null)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        await new SchemaMigrator(connection).Migrate(cancellationToken);
    }
}
