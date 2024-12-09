using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Document;

internal interface IStartUpDocumentMigrationRunner
{
    void RunAll(IMongoDatabase database);
}