using System.Data.Common;
using System.Threading.Tasks;

namespace ADatabaseFixture;

public interface IDatabaseAdapter
{
    /// <summary>
    /// Creates a new test database
    /// </summary>
    /// <returns>Connection string to the new database</returns>
    Task<string> CreateDatabase();

    /// <summary>
    /// Attempt to remove the database
    /// </summary>
    Task TryRemoveDatabase();

    /// <summary>
    /// Creates a new database connection
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>New connection</returns>
    DbConnection CreateNewConnection(string connectionString);
}
