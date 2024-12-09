using Mongo.Migration.Documents;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

public interface IDatabaseMigrationRunner
{
    void Run(IMongoDatabase db, DocumentVersion? targetVersion = null);
}