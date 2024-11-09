using Mongo.Migration.Documents.Serializers;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[SetUpFixture]
public class DatabaseMigrationRunnerSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var documentVersionSerializer = new DocumentVersionSerializer();
        BsonSerializer.TryRegisterSerializer(documentVersionSerializer.ValueType, documentVersionSerializer);
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        // Do logout here
    }
}