using ADatabaseFixture.ADatabaseMigrator.Tests.Core;
using Dapper;
using DataDude;
using Shouldly;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests;

public class ADatabaseMigratorTests(DatabaseFixture fixture) : DatabaseTest(fixture)
{
    [Fact]
    public async Task CanPersistAndFetchStaff()
    {
        using var connection = CreateNewConnection();

        await Dude
            .Insert("Department", new { Name = "IT" })
            .Insert("Person", new { Name = "John Doe" })
            .Insert("Employee")
            .Go(connection);

        var staff = await connection.QueryAsync<Staff>(@"SELECT * FROM Staff");

        staff.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            employee => employee.Name.ShouldBe("John Doe"),
            employee => employee.Department.ShouldBe("IT"));
    }

    [Fact]
    public async Task EnsureDatabaseIsEmptyForNewTests()
    {
        using var connection = CreateNewConnection();

        var staff = await connection.QueryAsync<Staff>(@"SELECT * FROM Staff");

        staff.ShouldBeEmpty();
    }

    private record Staff(int Id, string Name, string Department);
}
