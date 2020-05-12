using System;
using System.Data;

namespace ADatabaseFixture
{
    /// <summary>
    /// To be used for creating and initializing a database before tests and disposing the database afterwards
    /// To customize setup/teardown override <see cref="InitializeDatabase"/> and <see cref="Dispose"/>
    /// </summary>
    public abstract class DatabaseFixtureBase : IDisposable
    {
        private readonly IDatabaseAdapter _databaseAdapter;

        protected DatabaseFixtureBase(IDatabaseAdapter databaseAdapter, IMigrator? migrator = null)
        {
            Migrator = migrator ?? new NoOpMigrator();
            _databaseAdapter = databaseAdapter;
            ConnectionString = InitializeDatabase();
        }

        public string ConnectionString { get; }

        protected IMigrator Migrator { get; }

        /// <summary>
        /// Creates and opens a new connection
        /// </summary>
        public virtual IDbConnection CreateNewConnection()
        {
            var connection = _databaseAdapter.CreateNewConnection(ConnectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        public virtual void Dispose() => _databaseAdapter.TryRemoveDatabase();

        protected virtual string InitializeDatabase()
        {
            _databaseAdapter.TryRemoveDatabase();
            var connectionString = _databaseAdapter.CreateDatabase();
            Migrator.MigrateUp(connectionString);
            return connectionString;
        }

        public string[] MigratorVersioningTables => Migrator.VersioningTables;
    }
}
