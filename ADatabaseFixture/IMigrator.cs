using System.Threading.Tasks;

namespace ADatabaseFixture
{
    public interface IMigrator
    {
        Task MigrateUp(string connectionString);
    }
}
