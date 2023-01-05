using System.Data;
using System.Threading.Tasks;

namespace ADatabaseFixture
{
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Creates a new testabase
        /// </summary>
        /// <returns>Connection string to the new database</returns>
        ValueTask<string> CreateDatabase();

        /// <summary>
        /// Attempt to remove the database
        /// </summary>
        ValueTask TryRemoveDatabase();

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>New connection</returns>
        IDbConnection CreateNewConnection(string connectionString);
    }
}
