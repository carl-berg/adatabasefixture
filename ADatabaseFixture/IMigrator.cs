using System.Threading;
using System.Threading.Tasks;

namespace ADatabaseFixture;

public interface IMigrator
{
    Task MigrateUp(string connectionString, CancellationToken? cancellationToken = null);
}
