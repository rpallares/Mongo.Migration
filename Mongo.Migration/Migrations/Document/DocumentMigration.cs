using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document;

public abstract class DocumentMigration<TClass> : IDocumentMigration
    where TClass : IDocument
{
    protected DocumentMigration(string version)
    {
        Version = DocumentVersion.Parse(version.AsSpan());
    }

    public DocumentVersion Version { get; }

    public Type Type => typeof(TClass);

    public abstract void Up(BsonDocument document);

    public abstract void Down(BsonDocument document);
}