# ADatabaseFixture
An abstraction to help write integration tests. The idea is to have a `DatabaseFixture` that gets constructed once, used for all tests and disposed when the tests are done. How this is orchestrated varies slightly depending on which test framework you use. The fixture is responsible for creating an empty database and running migrations to get the database into a "testable" state and also disposing of the database when the tests are done.

The package contains the abstract class `DatabaseFixtureBase` which requires an `IDatabaseAdapter` and an optional `IMigrator` to be created.
This package also contains a default implementation of `IDatabaseAdapter` to for use with SqlServer databases: `SqlServerDatabaseAdapter`.


## Example setup using xUnit
In this example we will assume you have created a migrator class (`FixtureMigrator`). See instructions below on how to implement a migrator class using ADatabaseMigrator, EF Core or FluentMigrations.

1. Create your fixture class
```c#
public class DatabaseFixture() : DatabaseFixtureBase(
    new SqlServerDatabaseAdapter(ConnectionFactory), 
    new FixtureMigrator()), IAsyncLifetime
{
    private static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
}
```

2. Create a collection definition using your fixture (xUnit specific). The purpose for this is to only have your fixture created once and reused for all your integration tests

```c#
[CollectionDefinition("DatabaseIntegrationTest")]
public class DatabaseCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
```

3. Optional: Create a base class for your tests (optionally using [DataDude](https://github.com/carl-berg/data-dude) and [Respawn](https://github.com/jbogard/Respawn))
```c#
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
```c#
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

## Using schema migration libraries
ADatabaseFixture makes it easy for you to plug in your own schema migration logic. Here are some examples of how to interface with a few migration libraries. All you need is to create a class that implements the interface `IMigrator`.

### Example using [ADatabaseMigrator](https://github.com/carl-berg/ADatabaseMigrator)
This requires referencing the package [ADatabaseMigrator](https://www.nuget.org/packages/ADatabaseMigrator)
```c#
public class FixtureMigrator : ADatabaseFixture.IMigrator
{
    public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // See documentation at https://github.com/carl-berg/ADatabaseMigrator for how to create a ADatabaseMigrator migrator class
        await new MyDatabaseMigrator(connection).Migrate(cancellationToken);
    }
}
```

### Example using [EfCore Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations)
This requires referencing the package [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
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

### Example using [FluentMigrator](https://fluentmigrator.github.io)
This requires referencing the package [FluentMigrator.Runner](https://www.nuget.org/packages/FluentMigrator.Runner)
```c#
public class FixtureMigrator : ADatabaseFixture.IMigrator
{
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
```