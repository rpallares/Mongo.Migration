using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors;

internal sealed class MigrationBsonSerializerProvider : IRegistryAwareBsonSerializationProvider
{
    private static readonly Type s_migrationInterceptorGenericType = typeof(MigrationInterceptor<>);
    private static readonly Type s_iDocumentType = typeof(IDocument);

    private readonly IDocumentVersionService _documentVersionService;

    private readonly IDocumentMigrationRunner _migrationRunner;

    public MigrationBsonSerializerProvider(IDocumentMigrationRunner migrationRunner,
        IDocumentVersionService documentVersionService)
    {
        _migrationRunner = migrationRunner;
        _documentVersionService = documentVersionService;
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return GetSerializer(type, BsonSerializer.SerializerRegistry);
    }

    public IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
    {
        if (type is null || !ShouldBeMigrated(type))
        {
            return null!;
        }

        var genericType = s_migrationInterceptorGenericType.MakeGenericType(type);
        return (IBsonSerializer) Activator.CreateInstance(genericType, _migrationRunner, _documentVersionService)!;
    }

    private static bool ShouldBeMigrated(Type type)
    {
        return type.GetInterfaces().Contains(s_iDocumentType);
    }
}