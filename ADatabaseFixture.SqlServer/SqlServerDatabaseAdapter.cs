using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ADatabaseFixture
{
    public class SqlServerDatabaseAdapter : IDatabaseAdapter
    {
        private readonly string _databaseName;
        private readonly string _dataSource;
        private readonly string _auth;

        /// <summary>
        /// Construct an SqlServer Database manager
        /// </summary>
        /// <param name="databaseName">Database name (defaults to a unique database name if not specified)</param>
        /// <param name="dataSource">Data source (defaults to localdb)</param>
        /// <param name="auth">Authentication (defaults to integrated security)</param>
        public SqlServerDatabaseAdapter(
            string? databaseName = null, 
            string dataSource = @"(localdb)\MSSQLLocalDB", 
            string auth = "Integrated Security=True") 
        {
            _databaseName = databaseName ?? $"TestDatabase_{DateTime.Now:yyyy-MM-dd_HH-mm}";
            _dataSource = dataSource;
            _auth = auth;
        }

        /// <summary>
        /// Create a new database using specified database name
        /// </summary>
        public virtual string CreateDatabase()
        {
            var filePath = GetDatabasePath();

            string connectionString = GetMasterConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = $"CREATE DATABASE [{_databaseName}] ON (NAME = N'{_databaseName}', FILENAME = '{filePath}')";
                cmd.ExecuteNonQuery();

                return GetDatabaseConnectionString();
            }
        }

        public IDbConnection CreateNewConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Kills open connections, Drops database and tries to remove the file
        /// </summary>
        public virtual void TryRemoveDatabase()
        {
            KillOpenConnections();
            DropDatabase();
            TryRemoveDatabaseFile();
        }

        /// <summary>
        /// Attempts to drop database if it exists
        /// </summary>
        public virtual void DropDatabase()
        {
            string connectionString = GetMasterConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = string.Format($"DROP DATABASE IF EXISTS [{_databaseName}]");
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Attempts to kill any open connections
        /// </summary>
        public virtual void KillOpenConnections()
        {
            string connectionString = GetMasterConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = $@"
                    DECLARE @kill varchar(8000) = '';  
                    SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'  
                    FROM sys.dm_exec_sessions
                    WHERE database_id  = db_id('{_databaseName}')

                    EXEC(@kill);";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Attempts to remove database file if it exists
        /// </summary>
        public virtual void TryRemoveDatabaseFile()
        {
            var filePath = GetDatabasePath();
            if (new FileInfo(filePath) is FileInfo file && file.Exists)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Database file path
        /// </summary>
        public virtual string GetDatabasePath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(assembly.Location);
            return Path.Combine(path, $"{_databaseName}.mdf");
        }

        public virtual string GetMasterConnectionString()
        {
            return $@"Data Source={_dataSource};Initial Catalog=master;{_auth}";
        }

        public virtual string GetDatabaseConnectionString()
        {
            return $@"Data Source={_dataSource};Initial Catalog={_databaseName};{_auth}";
        }
    }
}
