namespace ADatabaseFixture
{
    public interface IMigrator
    {
        void MigrateUp(string connectionString);
    }
}
