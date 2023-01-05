using System.Threading.Tasks;

namespace ADatabaseFixture
{
    public interface IMigrator
    {
        ValueTask MigrateUp(string connectionString);
    }
}
