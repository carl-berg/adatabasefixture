using GalacticWasteManagement;
using System;

namespace ADatabaseFixture.GalacticWasteManagement
{
    public class GalacticWasteManagementMigrator : IMigrator
    {
        private readonly Func<string, GalacticWasteManager> _createManager;
        private readonly string _migrationMode;

        public static string[] VersioningTables => new[] { "SchemaVersionJournal" };

        internal GalacticWasteManagementMigrator(Func<string, GalacticWasteManager> createManager, string migrationMode)
        {
            _createManager = createManager;
            _migrationMode = migrationMode;
        }

        public void MigrateUp(string connectionString)
        {
            var migrator = _createManager(connectionString);
            var output = new DebugLogger();
            migrator.Logger = output;
            migrator.Output = output;
            migrator.Update(_migrationMode).GetAwaiter().GetResult();
        }

        public static IMigrator Create(
            IProjectSettings projectSettings,
            string migrationMode = "LiveField")
        {
            return new GalacticWasteManagementMigrator(
                connectionString => GalacticWasteManager.Create(projectSettings, connectionString),
                migrationMode);
        }

        public static IMigrator Create<AssemblyTypeContainingMigration>(
            Action<IProjectSettings>? configureProjectSettings = null,
            string migrationMode = "LiveField")
        {
            return new GalacticWasteManagementMigrator(
                connectionString => GalacticWasteManager.Create<AssemblyTypeContainingMigration>(connectionString, configureProjectSettings),
                migrationMode);
        }
    }
}
