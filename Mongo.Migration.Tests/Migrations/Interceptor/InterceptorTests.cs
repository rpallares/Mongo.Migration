﻿using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Interceptor;

public class InterceptorMigrationWhenCreating : IntegrationTest
{
    private const string DatabaseName = "TestDatabase";
    private const string CollectionName = "TestCollection";

    [Test]
    public async Task TestSerializationIntercepted()
    {
        IMongoCollection<BsonDocument> untypedCollection = GetCollection<BsonDocument>();
        IMongoCollection<TestDocumentWithTwoMigrationMiddleVersion> testDocumentCollection =
            GetCollection<TestDocumentWithTwoMigrationMiddleVersion>();

        await testDocumentCollection.InsertOneAsync(new TestDocumentWithTwoMigrationMiddleVersion
        {
            Doors1 = 42
        });

        BsonDocument? documentInserted = await untypedCollection
            .Find(Builders<BsonDocument>.Filter.Eq("Doors1", 42))
            .FirstOrDefaultAsync();

        Assert.That(documentInserted, Is.Not.Null);
        Assert.That(documentInserted["Version"].AsString, Is.EqualTo("0.0.1"));
    }

    [Test]
    public async Task TestDeserializationIntercepted()
    {
        IMongoCollection<BsonDocument> untypedCollection = GetCollection<BsonDocument>();
        IMongoCollection<TestDocumentWithTwoMigrationMiddleVersion> testDocumentCollection =
            GetCollection<TestDocumentWithTwoMigrationMiddleVersion>();

        await untypedCollection.InsertManyAsync(new[]
        {
            new BsonDocument
            {
                new("Doors0", new BsonInt32(0)),
                new("Version", new BsonString("0.0.0"))
            },
            new BsonDocument
            {
                new("Doors1", new BsonInt32(1)),
                new("Version", new BsonString("0.0.1"))
            },
            new BsonDocument
            {
                new("Doors2", new BsonInt32(2)),
                new("Version", new BsonString("0.0.2"))
            }
        });

        var asyncCursor = await testDocumentCollection
            .FindAsync(FilterDefinition<TestDocumentWithTwoMigrationMiddleVersion>.Empty);
        List<TestDocumentWithTwoMigrationMiddleVersion> documents = await asyncCursor.ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(documents.Count, Is.EqualTo(3));
            Assert.That(
                documents.Select(d => d.Version.ToString()),
                Is.All.EqualTo("0.0.1"));

            Assert.That(documents[0].Doors1, Is.EqualTo(0));
            Assert.That(documents[1].Doors1, Is.EqualTo(1));
            Assert.That(documents[2].Doors1, Is.EqualTo(2));
        });
    }

    [SetUp]
    public void SetUpLocal()
    {
        IMongoCollection<BsonDocument> _ = GetCollection<BsonDocument>();
    }

    [TearDown]
    public async Task TearDownLocalAsync()
    {
        await GetCollection<BsonDocument>()
            .DeleteManyAsync(d => true);
    }

    private static IMongoCollection<TDocument> GetCollection<TDocument>()
    {
        return TestcontainersContext.Provider.GetRequiredService<IMongoClient>()
            .GetDatabase(DatabaseName)
            .GetCollection<TDocument>(CollectionName);
    }
}