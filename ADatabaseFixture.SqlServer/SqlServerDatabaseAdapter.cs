using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace ADatabaseFixture;

/// <summary>
/// Construct an SqlServer Database manager
/// </summary>
/// <param name="connectionFactory">Connection factory, constructs a DbConnection given a connection string</param>
/// <param name="databaseName">Database name (defaults to a unique database name if not specified)</param>
/// <param name="dataSource">Data source (defaults to localdb)</param>
/// <param name="auth">Authentication (defaults to integrated security)</param>
public class SqlServerDatabaseAdapter(
    Func<string, DbConnection> connectionFactory,
    string? databaseName = null,
    string dataSource = @"(localdb)\MSSQLLocalDB",
    string auth = "Integrated Security=True") : IDatabaseAdapter
{
    public string DatabaseName { get; } = databaseName ?? $"TestDatabase_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";

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
        cmd.CommandText = $"CREATE DATABASE [{DatabaseName}] ON (NAME = N'{DatabaseName}', FILENAME = '{filePath}')";
        await cmd.ExecuteNonQueryAsync();
        return GetDatabaseConnectionString();
    }

    public DbConnection CreateNewConnection(string connectionString) => connectionFactory(connectionString);

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
    protected virtual async Task DropDatabase(DbConnection connection)
    {
#if NETSTANDARD2_1_OR_GREATER
        await using var cmd = connection.CreateCommand();
#else
        using var cmd = connection.CreateCommand();
#endif
        cmd.CommandText = $"DROP DATABASE IF EXISTS [{DatabaseName}]";
        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Attempts to kill any open connections
    /// </summary>
    public virtual async Task KillOpenConnections(DbConnection connection)
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
            WHERE database_id  = db_id('{DatabaseName}')
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

        return Path.Combine(path, $"{DatabaseName}.mdf");
    }

    public virtual string GetMasterConnectionString() => $@"Data Source={dataSource};Initial Catalog=master;{auth}";

    public virtual string GetDatabaseConnectionString() => $@"Data Source={dataSource};Initial Catalog={databaseName};{auth}";
}
