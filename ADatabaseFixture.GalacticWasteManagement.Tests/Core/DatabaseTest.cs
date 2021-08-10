using System.Threading.Tasks;
using CaptainData;
using CaptainData.Rules.PreDefined.Identity;
using Respawn;
using Xunit;

namespace ADatabaseFixture.GalacticWasteManagement.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest : IAsyncLifetime
    {
        public DatabaseTest(DatabaseFixture fixture)
        {
            Fixture = fixture;
            Captain = new Captain();
            Captain.AddRule(new SmartForeignKeyRule(new ForeignKeyNamingConvention()));
        }

        public DatabaseFixture Fixture { get; }
        public Captain Captain { get; }

        public static Checkpoint Checkpoint { get; } = new Checkpoint
        {
            TablesToIgnore = GalacticWasteManagementMigrator.VersioningTables,
        };

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync() => Checkpoint.Reset(Fixture.ConnectionString);

        private class ForeignKeyNamingConvention : ColumnSuffixMatchStrategy
        {
            public ForeignKeyNamingConvention() => EndsWith = "Id";
        }
    }
}
