using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using Mongo.Migration.Documents;

namespace Mongo.Migration.Bson;
internal abstract class BaseMigrationSerializer<TDocument> : SerializerBase<TDocument>
{
    private static readonly Type s_tDocumentType = typeof(TDocument);

    private readonly IDocumentMigrationRunner _migrationRunner;

    protected string VersionFieldName { get; }

    protected DocumentVersion RuntimeVersion { get; }

    protected string RuntimeVersionString { get; }

    protected BsonClassMapSerializer<TDocument> InnerSerializer { get; }

    protected BaseMigrationSerializer(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
    {
        _migrationRunner = migrationRunner;
        VersionFieldName = documentVersionService.GetVersionFieldName();
        RuntimeVersion = documentVersionService.GetCurrentOrLatestMigrationVersion(s_tDocumentType);
        RuntimeVersionString = RuntimeVersion.ToString();
        BsonClassMap classMap = BsonClassMap.LookupClassMap(s_tDocumentType);
        InnerSerializer = new BsonClassMapSerializer<TDocument>(classMap);
    }

    public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        BsonReaderBookmark bookmark = context.Reader.GetBookmark();

        context.Reader.ReadStartDocument();
        string currentVersion = context.Reader.FindStringElement(VersionFieldName);
        context.Reader.ReturnToBookmark(bookmark);

        if (RuntimeVersionString == currentVersion)
        {
            return InnerSerializer.Deserialize(context, args);
        }

        BsonDocument document = BsonDocumentSerializer.Instance.Deserialize(context);
        _migrationRunner.Run(s_tDocumentType, document, RuntimeVersion);
        var migratedContext = BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));
        return InnerSerializer.Deserialize(migratedContext, args);
    }
}
