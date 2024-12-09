using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors;

public interface IMigrationInterceptorFactory
{
    IBsonSerializer Create(Type type);

    IDocumentVersionService DocumentVersionService { get; }

    IDocumentMigrationRunner MigrationRunner { get; }
}