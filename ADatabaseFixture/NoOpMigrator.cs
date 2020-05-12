namespace ADatabaseFixture
{
    public class NoOpMigrator : IMigrator
    {
        public string[] VersioningTables => new string[0];

        public void MigrateUp(string connectionString) { }
    }
}
