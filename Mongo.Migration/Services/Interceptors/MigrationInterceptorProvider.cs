using Mongo.Migration.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors;

internal class MigrationInterceptorProvider : IMigrationInterceptorProvider
{
    private static readonly Type s_iDocumentType = typeof(IDocument);
    private static readonly Type s_bsonDocumentType = typeof(BsonDocument);

    private readonly IMigrationInterceptorFactory _migrationInterceptorFactory;

    public MigrationInterceptorProvider(IMigrationInterceptorFactory migrationInterceptorFactory)
    {
        _migrationInterceptorFactory = migrationInterceptorFactory;
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return GetSerializer(type, BsonSerializer.SerializerRegistry);
    }

    public IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
    {
        if (ShouldBeMigrated(type))
        {
            return _migrationInterceptorFactory.Create(type);
        }

        // Go to next provider
        // see https://github.com/mongodb/mongo-csharp-driver/blob/main/src/MongoDB.Bson/Serialization/BsonSerializerRegistry.cs#L155
        return null!;
    }

    private static bool ShouldBeMigrated(Type type)
    {
        return type != s_bsonDocumentType
            && type.GetInterfaces().Contains(s_iDocumentType);
    }

    public IMigrationInterceptorFactory MigrationInterceptorFactory => _migrationInterceptorFactory;
}