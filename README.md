# ADatabaseFixture
An abstraction to help write integration tests. The idea is to have a `DatabaseFixture` that gets constructed once, used for all tests and disposed when the tests are done. How this is orchestrated varies slightly depending on which test framework you use. The fixture is responsible for creating an empty database and running migrations to get the database into a "testable" state and also disposing of the database when the tests are done.

Contains the following packages:

**ADatabaseFixture**
Contains the abstract class DatabaseFixtureBase which needs an `IDatabaseAdapter` and an optional `IMigrator` to be created.

**ADatabaseFixture.SqlServer**
Contains `SqlServerDatabaseAdapter : IDatabaseAdapter` for usage against an SqlServer database

**ADatabaseFixture.GalacticWasteManagement**
Contains `GalacticWasteManagementMigrator : IMigrator` which implements the migrator using the [Galactic-Waste-Management](https://github.com/mattiasnordqvist/Galactic-Waste-Management) migration library

**ADatabaseFixture.FluentMigrator**
Contains `FluentMigratorMigrator : IMigrator` which implements the migrator using the [FluentMigrator](https://fluentmigrator.github.io/) migration library

## Example setup using xUnit
In this example we will use the GalacticWasteManagement migrator as an example, but the setup would be almost identical for FluentMigrator.

1. Create your fixture class
```csharp
public class DatabaseFixture : DatabaseFixtureBase
{
    public DatabaseFixture() : base(
            new SqlServerDatabaseAdapter(), 
            GalacticWasteManagementMigrator.Create<AssemblyTypeContainingMigration>())
    {
    }
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

3. Optional: Create a base class for your tests (optionally using CaptainData and Respawn)
```csharp
[Collection("DatabaseIntegrationTest")]
public abstract class DatabaseTest : IAsyncLifetime
{
    public DatabaseTest(DatabaseFixture fixture)
    {
        Fixture = fixture;
        Captain = new Captain();
    }

    public DatabaseFixture Fixture { get; }
    public Captain Captain { get; }

    public static Checkpoint Checkpoint { get; } = new Checkpoint
    {
        TablesToIgnore = GalacticWasteManagementMigrator.VersioningTables,
    };

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Checkpoint.Reset(Fixture.ConnectionString);
}
```

4. Go ahead and write your first integration test
```csharp
public class Mytest : DatabaseTest
{
    public Mytest(DatabaseFixture fixture) : base(fixture) { }

    [Fact]
    public async Task TestChangeDepartment()
    {
        // Arrange
        var connection = Fixture.CreateNewConnection();

        await Captain
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

## Example setup using nUnit
// TODO

## Release Notes

### 0.1.0
Initial version

### 0.1.1
Added migrator versioning tables

### 0.1.2
Made migrator versioning tables static instead to be able to use with Respawn checkpoint (which should be a static instance)