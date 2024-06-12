using System.Threading;
using System.Threading.Tasks;

namespace ADatabaseFixture;

internal class NoOpMigrator : IMigrator
{
    public Task MigrateUp(string connectionString, CancellationToken? cancellationToken = null) => Task.CompletedTask;
}
