using CaptainData;
using CaptainData.Rules.PreDefined.Identity;
using Respawn;
using System;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest : IDisposable
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
            TablesToIgnore = FluentMigratorMigrator.VersioningTables,
        };

        public void Dispose()
        {
            Checkpoint.Reset(Fixture.ConnectionString).GetAwaiter().GetResult();
        }

        private class ForeignKeyNamingConvention : ColumnSuffixMatchStrategy
        {
            public ForeignKeyNamingConvention() => EndsWith = "Id";
        }
    }
}
