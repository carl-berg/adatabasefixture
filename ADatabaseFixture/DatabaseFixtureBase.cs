using System;
using System.Data;
using System.Threading.Tasks;

namespace ADatabaseFixture
{
    /// <summary>
    /// To be used for creating and initializing a database before tests and disposing the database afterwards
    /// To customize setup/teardown override <see cref="InitializeAsync"/> and <see cref="DisposeAsync"/>
    /// </summary>
    public abstract class DatabaseFixtureBase
    {
        private readonly IDatabaseAdapter _databaseAdapter;
        private string? _connectionString;

        protected DatabaseFixtureBase(IDatabaseAdapter databaseAdapter, IMigrator? migrator = null)
        {
            Migrator = migrator ?? new NoOpMigrator();
            _databaseAdapter = databaseAdapter;
        }

        public virtual string ConnectionString  => _connectionString ?? throw new Exception($"Database fixture has not been initialized. Ensure {nameof(InitializeAsync)} have been called before creating a connection");

        protected virtual IMigrator Migrator { get; }

        /// <summary>
        /// Creates a new open connection
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

        /// <summary>
        /// Ensure database is initialized and ready
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await _databaseAdapter.TryRemoveDatabase();
            _connectionString = await _databaseAdapter.CreateDatabase();
            await Migrator.MigrateUp(ConnectionString);
        }

        /// <summary>
        /// Disposes the database and removes resources
        /// </summary>
        /// <returns></returns>
        public Task DisposeAsync() => _databaseAdapter.TryRemoveDatabase();
    }
}
