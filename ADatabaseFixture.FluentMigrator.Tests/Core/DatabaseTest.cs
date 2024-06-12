using System.Threading.Tasks;
using DataDude;
using Respawn;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core;

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

    private static Respawner Respawner { get; set; }

    public async Task InitializeAsync()
    {
        Respawner ??= await Respawner.CreateAsync(Fixture.ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = [FixtureMigrator.VersioningTable],
        });
    }

    public Task DisposeAsync() => Respawner.ResetAsync(Fixture.ConnectionString);
}
