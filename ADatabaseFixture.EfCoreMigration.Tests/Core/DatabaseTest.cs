using System.Data.Common;
using ADatabaseFixture.EfCoreMigration.Tests.Core;
using ADatabaseFixture.EfCoreMigration.Tests.Database;
using ADatabaseMigrator.Journaling;
using DataDude;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Xunit;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Core;

[Collection("DatabaseIntegrationTest")]
public abstract class DatabaseTest(DatabaseFixture fixture) : IAsyncLifetime
{
    public Dude Dude { get; } = new Dude().EnableAutomaticForeignKeys();

    private static Respawner? Respawner { get; set; }

    protected DbConnection CreateNewConnection() => fixture.CreateNewConnection();
    protected StaffDbContext CreateDbContext(DbConnection connection) => FixtureMigrator.CreateDbContext(connection);

    public async Task InitializeAsync()
    {
        Respawner ??= await Respawner.CreateAsync(fixture.ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = [new(MigrationScriptJournalManager.JournalTableName)],
        });
    }

    public Task DisposeAsync() => Respawner?.ResetAsync(fixture.ConnectionString) ?? Task.CompletedTask;
}
