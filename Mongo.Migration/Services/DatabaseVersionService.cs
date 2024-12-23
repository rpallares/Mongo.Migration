using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Driver;

namespace Mongo.Migration.Services;

internal class DatabaseVersionService : IDatabaseVersionService
{
    private const string MigrationsCollectionName = "_migrations";

    private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;

    public DatabaseVersionService(IDatabaseTypeMigrationDependencyLocator migrationLocator)
    {
        _migrationLocator = migrationLocator;
    }

    public DocumentVersion GetLatestMigrationVersion()
    {
        return _migrationLocator.GetLatestVersion(typeof(DatabaseMigration));
    }

    public DocumentVersion GetLatestDatabaseVersion(IMongoDatabase db)
    {
        var migrations = GetMigrationsCollection(db).Find(m => true).ToList();
        if (migrations is { Count: 0 })
        {
            return DocumentVersion.Default;
        }

        return migrations.Max(m => m.Version);
    }

    public void Save(IMongoDatabase db, IDatabaseMigration migration)
    {
        GetMigrationsCollection(db).InsertOne(
            new()
            {
                MigrationId = migration.GetType().ToString(),
                Version = migration.Version
            });
    }

    public void Remove(IMongoDatabase db, IDatabaseMigration migration)
    {
        GetMigrationsCollection(db).DeleteOne(Builders<MigrationHistory>.Filter.Eq(mh => mh.MigrationId, migration.GetType().ToString()));
    }

    private static IMongoCollection<MigrationHistory> GetMigrationsCollection(IMongoDatabase db)
    {
        return db.GetCollection<MigrationHistory>(MigrationsCollectionName);
    }
}