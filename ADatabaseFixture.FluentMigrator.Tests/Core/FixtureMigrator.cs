using System.Threading;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace ADatabaseFixture.FluentMigrator.Tests.Core;

public class FixtureMigrator : IMigrator
{
    public const string VersioningTable = "VersionInfo";

    public Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
    {
        using var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                rb.ScanIn(typeof(FixtureMigrator).Assembly).For.Migrations();
                rb.WithGlobalConnectionString(connectionString);
                rb.AddSqlServer2016();
            })
            .BuildServiceProvider(false);

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        
        return Task.CompletedTask;
    }
}
