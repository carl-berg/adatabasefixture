using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ADatabaseFixture.EfCoreMigration.Tests.Database;

/// <summary>
/// A context factory that can be used to create the context by migration tool commands like:
/// > dotnet ef migrations add
/// > dotnet ef database update
/// </summary>
internal class StaffDbContextFactory : IDesignTimeDbContextFactory<StaffDbContext>
{
    public StaffDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StaffDbContext>();
        optionsBuilder.UseSqlServer();
        return new StaffDbContext(optionsBuilder.Options);
    }
}
