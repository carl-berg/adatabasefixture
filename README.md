# ADatabaseFixture
An abstraction to help write integration tests. The idea is to have a `DatabaseFixture` that gets constructed once, used for all tests and disposed when the tests are done. How this is orchestrated varies slightly depending on which test framework you use. The fixture is responsible for creating an empty database and running migrations to get the database into a "testable" state and also disposing of the database when the tests are done.

Contains the following packages:

**ADatabaseFixture**
Contains the abstract class DatabaseFixtureBase which needs an `IDatabaseAdapter` and an optional `IMigrator` to be created.
This package also contains to implementations of these interfaces: 
- `SqlServerDatabaseAdapter` (can be used when targeting SqlServer databases)
- `NoOpMigrator` (default migrator which simply does nothing)

**ADatabaseFixture.FluentMigrator**
Contains `FluentMigratorMigrator : IMigrator` which implements the migrator using the [FluentMigrator](https://fluentmigrator.github.io/) migration library

## Example setup using xUnit
In this example we will use the FluentMigrator migrator as an example (see examples below for integration with other libraries).

1. Create your fixture class
```csharp
public class DatabaseFixture() : DatabaseFixtureBase(
    new SqlServerDatabaseAdapter(ConnectionFactory), 
    FluentMigratorMigrator.Create<CreatePersonTable>(Database.SqlServer2016)), IAsyncLifetime
{
    private static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
}
```

2. Create a collection definition using your fixture (xUnit specific). The purpose for this is to only have your fixture created once and reused for all your integration tests

```csharp
[CollectionDefinition("DatabaseIntegrationTest")]
public class DatabaseCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
```

3. Optional: Create a base class for your tests (optionally using [DataDude](https://github.com/carl-berg/data-dude) and [Respawn](https://github.com/jbogard/Respawn))
```csharp
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
            TablesToIgnore = FluentMigratorMigrator.VersioningTables.Select(t => new Respawn.Graph.Table(t)).ToArray(),
        });
    }

    public Task DisposeAsync() => Respawner.ResetAsync(Fixture.ConnectionString);
}
```

4. Go ahead and write your first integration test (using [Dapper](https://github.com/DapperLib/Dapper) for example)
```csharp
public class Mytest : DatabaseTest
{
    public Mytest(DatabaseFixture fixture) : base(fixture) { }

    [Fact]
    public async Task TestChangeDepartment()
    {
        // Arrange
        using var connection = Fixture.CreateNewConnection();

        await Dude
            .Insert("Department", new { Id = 1, Name = "HR" })
            .Insert("Department", new { Id = 2, Name = "IT" })
            .Insert("Employee", new { Id = 1, Name = "Jane Doe", DepartmentId = 1 })
            .Go(connection);

        var handler = new ChangeDepartmentHandler(connection);
        var command = new ChangeDepartment(employee: 1, newDepartment: 2);

        // Act
        await handler.Handle(command);

        // Assert
        var departmentName = await connection.QuerySingleAsync<string>(@"
            SELECT Department.Name
            FROM Employee
            INNER JOIN Department ON Department.Id = Employee.DepartmentId");

        departmentName.ShouldBe("IT");
    }
}
```

## Using other migration libraries
ADatabaseFixture could easily be made compatible with other migration libraries like [ADatabaseMigrator](https://github.com/carl-berg/ADatabaseMigrator) or [EfCore Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations) without the need to add extra packages.

Here is an example of how you could create a fixture migrator for **EF Core**
```c#
public class FixtureMigrator : ADatabaseFixture.IMigrator
{
    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        using var dbContext = MyDbContext(new DbContextOptionsBuilder<MyDbContext>().UseSqlServer(connection).Options);
        connection.Open();
        await dbContext.Database.MigrateAsync(cancellationToken ?? default);
    }
}
```

.. and here's an example of how you would create a fixture migrator for **ADatabaseMigrator**
```c#
public class FixtureMigrator : ADatabaseFixture.IMigrator
{
    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // See documentation at https://github.com/carl-berg/ADatabaseMigrator for how to create a migrator class
        await new ADatabaseMigrator(connection).Migrate(cancellationToken);
    }
}
```