using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Services.Interceptors;

internal sealed class MigrationInterceptor<TDocument> : SerializerBase<TDocument>
    where TDocument : IDocument
{
    private static readonly Type s_tDocumentType = typeof(TDocument);

    private readonly IDocumentMigrationRunner _migrationRunner;

    private readonly DocumentVersion _runtimeVersion;

    private readonly BsonClassMapSerializer<TDocument> _innerSerializer;

    public MigrationInterceptor(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
    {
        _migrationRunner = migrationRunner;
        _runtimeVersion = documentVersionService.GetCurrentOrLatestMigrationVersion(s_tDocumentType);
        BsonClassMap classMap = BsonClassMap.LookupClassMap(s_tDocumentType);
        _innerSerializer = new BsonClassMapSerializer<TDocument>(classMap);
    }

    public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        BsonDocument document = BsonDocumentSerializer.Instance.Deserialize(context);
        _migrationRunner.Run(s_tDocumentType, document, _runtimeVersion);
        var migratedContext = BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));
        return _innerSerializer.Deserialize(migratedContext, args);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDocument value)
    {
        value.Version = _runtimeVersion;
        _innerSerializer.Serialize(context, args, value);
    }
}