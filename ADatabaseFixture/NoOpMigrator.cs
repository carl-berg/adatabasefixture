using System;
using System.Threading.Tasks;

namespace ADatabaseFixture
{
    public class NoOpMigrator : IMigrator
    {
        public string[] VersioningTables => Array.Empty<string>();

        public ValueTask MigrateUp(string connectionString) => ValueTask.CompletedTask;
    }
}
