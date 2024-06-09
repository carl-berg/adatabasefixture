using ADatabaseFixture.ADatabaseMigrator.Tests.Database;
using ADatabaseMigrator.Journaling;
using Microsoft.Data.SqlClient;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Core;

public class FixtureMigrator : IMigrator
{
    public const string VersioningTable = MigrationScriptJournalManager.JournalTableName;

    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken = null)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        await new SchemaMigrator(connection).Migrate(cancellationToken);
    }
}
