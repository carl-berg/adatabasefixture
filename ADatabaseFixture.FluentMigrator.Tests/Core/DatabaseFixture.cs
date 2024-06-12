using System;
using ADatabaseFixture.FluentMigrator.Tests.Database.Migrations;
using Microsoft.Data.SqlClient;
using Xunit;

namespace ADatabaseFixture.FluentMigrator.Tests.Core;

public class DatabaseFixture() : DatabaseFixtureBase(
    new SqlServerDatabaseAdapter(ConnectionFactory, DatabaseName()),
    new FixtureMigrator()), IAsyncLifetime
{
    private static string DatabaseName() => $"FluentMigrator_TestDatabase_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";

    private static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
}
