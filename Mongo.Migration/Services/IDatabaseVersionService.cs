using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;

using MongoDB.Driver;

namespace Mongo.Migration.Services;

public interface IDatabaseVersionService
{
    DocumentVersion GetLatestMigrationVersion();

    DocumentVersion GetLatestDatabaseVersion(IMongoDatabase db);

    void Save(IMongoDatabase db, IDatabaseMigration migration);

    void Remove(IMongoDatabase db, IDatabaseMigration migration);
}