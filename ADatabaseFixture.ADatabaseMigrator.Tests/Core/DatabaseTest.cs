using System.Data.Common;
using ADatabaseMigrator.Journaling;
using DataDude;
using Respawn;
using Xunit;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Core;

[Collection("DatabaseIntegrationTest")]
public abstract class DatabaseTest(DatabaseFixture fixture) : IAsyncLifetime
{
    public Dude Dude { get; } = new Dude().EnableAutomaticForeignKeys();

    private static Respawner? Respawner { get; set; }
    protected DbConnection CreateNewConnection() => fixture.CreateNewConnection();

    public async Task InitializeAsync()
    {
        Respawner ??= await Respawner.CreateAsync(fixture.ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = [FixtureMigrator.VersioningTable],
        });
    }

    public Task DisposeAsync() => Respawner?.ResetAsync(fixture.ConnectionString) ?? Task.CompletedTask;
}
