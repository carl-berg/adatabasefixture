using GalacticWasteManagement;
using System;
using System.Threading.Tasks;

namespace ADatabaseFixture.GalacticWasteManagement
{
    public class GalacticWasteManagementMigrator : IMigrator
    {
        private readonly Func<string, GalacticWasteManager> _createManager;
        private readonly Action<GalacticWasteManager>? _configureManager;
        private readonly string _migrationMode;

        public static string[] VersioningTables => new[] { "SchemaVersionJournal" };

        internal GalacticWasteManagementMigrator(Func<string, GalacticWasteManager> createManager, string migrationMode, Action<GalacticWasteManager>? configureManager = null)
        {
            _createManager = createManager;
            _migrationMode = migrationMode;
            _configureManager = configureManager;
        }

        public async Task MigrateUp(string connectionString)
        {
            var migrator = _createManager(connectionString);
            var output = new DebugLogger();
            migrator.Logger = output;
            migrator.Output = output;
            _configureManager?.Invoke(migrator);
            await migrator.Update(_migrationMode);
        }

        public static IMigrator Create(
            IProjectSettings projectSettings,
            string migrationMode = "LiveField",
            Action<GalacticWasteManager>? configureManager = null)
        {
            return new GalacticWasteManagementMigrator(
                connectionString => GalacticWasteManager.Create(projectSettings, connectionString),
                migrationMode,
                configureManager);
        }

        public static IMigrator Create<AssemblyTypeContainingMigration>(
            Action<IProjectSettings>? configureProjectSettings = null,
            string migrationMode = "LiveField",
            Action<GalacticWasteManager>? configureManager = null)
        {
            return new GalacticWasteManagementMigrator(
                connectionString => GalacticWasteManager.Create<AssemblyTypeContainingMigration>(connectionString, configureProjectSettings),
                migrationMode,
                configureManager);
        }
    }
}
