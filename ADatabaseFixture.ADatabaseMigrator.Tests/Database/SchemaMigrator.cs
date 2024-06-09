using System.Data.Common;
using ADatabaseMigrator;
using ADatabaseMigrator.Hashing;
using ADatabaseMigrator.Journaling;
using ADatabaseMigrator.ScriptLoading.EmbeddedResources;
using ADatabaseMigrator.ScriptLoading.EmbeddedResources.Versioning;

namespace ADatabaseFixture.ADatabaseMigrator.Tests.Database;

public class SchemaMigrator(DbConnection connection) : Migrator(
    scriptLoader: new EmbeddedResourceScriptLoader(new MD5ScriptHasher(), config => config
        .UsingAssemblyFromType<SchemaMigrator>()
            .AddNamespaces<VersionFromPathVersionLoader>(MigrationScriptRunType.RunOnce, "Database.Scripts.Migrations")
            .AddNamespaces<VersionFromAssemblyVersionLoader>(MigrationScriptRunType.RunIfChanged, "Database.Scripts.RunIfChanged")),
    journalManager: new MigrationScriptJournalManager(connection),
    scriptRunner: new MigrationScriptRunner(connection));
