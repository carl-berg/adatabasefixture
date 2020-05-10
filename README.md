# ADatabaseFixture
An abstraction to help write integration tests. Contains the following packages:

## ADatabaseFixture
Contains the abstract class DatabaseFixtureBase which needs an `IDatabaseAdapter` and an optional `IMigrator` to be created.

## ADatabaseFixture.SqlServer
Contains `SqlServerDatabaseAdapter : IDatabaseAdapter` for usage against an SqlServer database. The class does not require any parameters, but these optional parameters can be provided:
- `databaseName`: Defaults to a unique time-based name. Override to provide your own database name
- `dataSource`: Defaults to `localdb`. Override to set your own data source
- `auth`: Defaults to `Integrated Security`. Override to set your own authentication

## ADatabaseFixture.GalacticWasteManagement
Contains `GalacticWasteManagementMigrator : IMigrator` which implements the migrator using the [Galactic-Waste-Management](https://github.com/mattiasnordqvist/Galactic-Waste-Management) migration library

## ADatabaseFixture.FluentMigrator
Contains `FluentMigratorMigrator : IMigrator` which implements the migrator using the [FluentMigrator](https://fluentmigrator.github.io/) migration library

## Using these packages for integration tests
TODO


## Release Notes

### 0.1.0
Initial version