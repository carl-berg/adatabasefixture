namespace ADatabaseFixture
{
    public class NoOpMigrator : IMigrator
    {
        public void MigrateUp(string connectionString) { }
    }
}
