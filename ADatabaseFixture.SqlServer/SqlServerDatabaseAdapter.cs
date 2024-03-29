﻿using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

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
        public virtual async Task<string> CreateDatabase()
        {
            var filePath = GetDatabasePath();
            string connectionString = GetMasterConnectionString();
#if NETSTANDARD2_1_OR_GREATER
            await using var connection = CreateNewConnection(connectionString);
# else
            using var connection = CreateNewConnection(connectionString);
#endif
            await connection.OpenAsync();
#if NETSTANDARD2_1_OR_GREATER
            await using var cmd = connection.CreateCommand();
#else
            using var cmd = connection.CreateCommand();
#endif
            cmd.CommandText = $"CREATE DATABASE [{_databaseName}] ON (NAME = N'{_databaseName}', FILENAME = '{filePath}')";
            await cmd.ExecuteNonQueryAsync();
            return GetDatabaseConnectionString();
        }

        public virtual SqlConnection CreateNewConnection(string connectionString) => new(connectionString);
        DbConnection IDatabaseAdapter.CreateNewConnection(string connectionString) => CreateNewConnection(connectionString);

        /// <summary>
        /// Kills open connections, Drops database and tries to remove the file
        /// </summary>
        public virtual async Task TryRemoveDatabase()
        {
            string connectionString = GetMasterConnectionString();
#if NETSTANDARD2_1_OR_GREATER
            await using var connection = CreateNewConnection(connectionString);
#else
            using var connection = CreateNewConnection(connectionString);
#endif
            await connection.OpenAsync();
            await KillOpenConnections(connection);
            await DropDatabase(connection);
            TryRemoveDatabaseFile();
        }

        /// <summary>
        /// Attempts to drop database if it exists
        /// </summary>
        protected virtual async Task DropDatabase(SqlConnection connection)
        {
#if NETSTANDARD2_1_OR_GREATER
            await using var cmd = connection.CreateCommand();
#else
            using var cmd = connection.CreateCommand();
#endif
            cmd.CommandText = $"DROP DATABASE IF EXISTS [{_databaseName}]";
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Attempts to kill any open connections
        /// </summary>
        public virtual async Task KillOpenConnections(SqlConnection connection)
        {
#if NETSTANDARD2_1_OR_GREATER
            await using var cmd = connection.CreateCommand();
#else
            using var cmd = connection.CreateCommand();
#endif
            cmd.CommandText = 
                $"""
                DECLARE @kill varchar(8000) = '';  
                SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'  
                FROM sys.dm_exec_sessions
                WHERE database_id  = db_id('{_databaseName}')
                AND is_user_process = 1;
                EXEC(@kill);
                """;
            await cmd.ExecuteNonQueryAsync();
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
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(appDataPath, "ADatabaseFixture");
            if (Directory.Exists(path) is false)
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, $"{_databaseName}.mdf");
        }

        public virtual string GetMasterConnectionString() => $@"Data Source={_dataSource};Initial Catalog=master;{_auth}";

        public virtual string GetDatabaseConnectionString() => $@"Data Source={_dataSource};Initial Catalog={_databaseName};{_auth}";
    }
}
