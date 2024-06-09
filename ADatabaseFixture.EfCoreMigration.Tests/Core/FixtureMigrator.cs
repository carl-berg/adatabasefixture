using ADatabaseFixture.EfCoreMigration.Tests.Database;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ADatabaseFixture.EfCoreMigration.Tests.Core;

public class FixtureMigrator : IMigrator
{
    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        using var dbContext = CreateDbContext(connection);
        connection.Open();
        await dbContext.Database.MigrateAsync(cancellationToken ?? default);
    }

    public static StaffDbContext CreateDbContext(DbConnection connection)
        => new(new DbContextOptionsBuilder<StaffDbContext>().UseSqlServer(connection).Options);
}
