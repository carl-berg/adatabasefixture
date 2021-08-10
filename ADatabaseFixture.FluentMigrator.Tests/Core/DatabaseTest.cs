using System.Threading.Tasks;
using DataDude;
using Respawn;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest : IAsyncLifetime
    {
        public DatabaseTest(DatabaseFixture fixture)
        {
            Fixture = fixture;
            Dude = new Dude().EnableAutomaticForeignKeys();
        }

        public DatabaseFixture Fixture { get; }
        public Dude Dude { get; }

        public static Checkpoint Checkpoint { get; } = new Checkpoint
        {
            TablesToIgnore = FluentMigratorMigrator.VersioningTables,
        };

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync() => Checkpoint.Reset(Fixture.ConnectionString);
    }
}
