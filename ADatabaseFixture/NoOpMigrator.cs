using System;
using System.Threading.Tasks;

namespace ADatabaseFixture
{
    public class NoOpMigrator : IMigrator
    {
        public string[] VersioningTables => Array.Empty<string>();

        public Task MigrateUp(string connectionString) => Task.CompletedTask;
    }
}
