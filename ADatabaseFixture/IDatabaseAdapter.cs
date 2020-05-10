using System.Data;

namespace ADatabaseFixture
{
    public interface IDatabaseAdapter
    {
        string CreateDatabase();
        void TryRemoveDatabase();
        IDbConnection CreateNewConnection(string connectionString);
    }
}
