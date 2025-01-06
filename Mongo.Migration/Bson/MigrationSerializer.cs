using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Mongo.Migration.Services;

namespace Mongo.Migration.Bson;
internal sealed class MigrationSerializer<TDocument> : SerializerBase<TDocument>
    where TDocument : IDocument
{
    private static readonly Type s_tDocumentType = typeof(TDocument);

    private readonly IDocumentMigrationRunner _migrationRunner;

    private readonly IDocumentVersionService _documentVersionService;

    private readonly DocumentVersion _runtimeVersion;

    private readonly string _runtimeVersionString;

    private readonly BsonClassMapSerializer<TDocument> _innerSerializer;

    public MigrationSerializer(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
    {
        _migrationRunner = migrationRunner;
        _documentVersionService = documentVersionService;
        _runtimeVersion = _documentVersionService.GetCurrentOrLatestMigrationVersion(s_tDocumentType);
        _runtimeVersionString = _runtimeVersion.ToString();
        BsonClassMap classMap = BsonClassMap.LookupClassMap(s_tDocumentType);
        _innerSerializer = new BsonClassMapSerializer<TDocument>(classMap);
    }

    public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        BsonReaderBookmark bookmark = context.Reader.GetBookmark();

        context.Reader.ReadStartDocument();
        string currentVersion = context.Reader.FindStringElement(_documentVersionService.GetVersionFieldName());
        context.Reader.ReturnToBookmark(bookmark);

        if (_runtimeVersionString == currentVersion)
        {
            return _innerSerializer.Deserialize(context, args);
        }

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
