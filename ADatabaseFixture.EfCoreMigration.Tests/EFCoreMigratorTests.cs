using ADatabaseFixture.ADatabaseMigrator.Tests.Core;
using ADatabaseFixture.EfCoreMigration.Tests.Core;
using DataDude;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace ADatabaseFixture.EfCoreMigration.Tests;

public class EFCoreMigratorTests(DatabaseFixture fixture) : DatabaseTest(fixture)
{
    [Fact]
    public async Task CanPersistAndFetchStaff()
    {
        using var connection = CreateNewConnection();
        using var dbContext = CreateDbContext(connection);

        await Dude
            .Insert("Department", new { Name = "IT" })
            .Insert("Person", new { Name = "John Doe" })
            .Insert("Employee")
            .Go(connection);

        var employees = await dbContext.Employees
            .Include(x => x.Person)
            .Include(x => x.Department)
            .ToListAsync();

        employees.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            employee => employee.Person.ShouldNotBeNull().Name.ShouldBe("John Doe"),
            employee => employee.Department.ShouldNotBeNull().Name.ShouldBe("IT"));
    }

    [Fact]
    public async Task EnsureDatabaseIsEmptyForNewTests()
    {
        using var connection = CreateNewConnection();
        using var dbContext = CreateDbContext(connection);

        var employees = await dbContext.Employees.ToListAsync();

        employees.ShouldBeEmpty();
    }
}
