using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Bson;

internal sealed class MigrationBsonSerializerProvider : IRegistryAwareBsonSerializationProvider
{
    private static readonly Type s_migrationSerializerGenericType = typeof(MigrationSerializer<>);
    private static readonly Type s_iDocumentType = typeof(IDocument);

    private readonly object[] _constructorParameters;

    public MigrationBsonSerializerProvider(IDocumentMigrationRunner migrationRunner,
        IDocumentVersionService documentVersionService)
    {
        _constructorParameters = new object[] { migrationRunner, documentVersionService };
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return GetSerializer(type, BsonSerializer.SerializerRegistry);
    }

    public IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
    {
        if (!ShouldBeMigrated(type))
        {
            return null!;
        }

        var genericType = s_migrationSerializerGenericType.MakeGenericType(type);
        return (IBsonSerializer)Activator.CreateInstance(genericType, _constructorParameters)!;
    }

    private static bool ShouldBeMigrated(Type type)
    {
        return type.GetInterfaces().Contains(s_iDocumentType);
    }
}