using System.Threading;
using System.Threading.Tasks;

namespace ADatabaseFixture;

public class NoOpMigrator : IMigrator
{
    public static string[] VersioningTables => [];

    public Task MigrateUp(string connectionString, CancellationToken? cancellationToken = null) => Task.CompletedTask;
}
