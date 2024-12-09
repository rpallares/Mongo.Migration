using System.ComponentModel.Design;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Performance;

[TestFixture]
public class PerformanceTestOnStartup
{
    private const int DocumentCount = 10000;

    private const string DatabaseName = "PerformanceTest";

    private const string CollectionName = "Test";

    private const int ToleranceMs = 2800;

    [Test]
    public void When_migrating_number_of_documents()
    {
        // Arrange
        // Worm up MongoCache
        ClearCollection();
        AddDocumentsToCache();
        ClearCollection();

        // Act
        // Measure time of MongoDb processing without Mongo.Migration
        InsertMany(DocumentCount, false);
        var sw = new Stopwatch();
        sw.Start();
        MigrateAll(false);
        sw.Stop();

        ClearCollection();

        // Measure time of MongoDb processing with Mongo.Migration
        IMongoClient client = TestcontainersContext.MongoClient;
        InsertMany(DocumentCount, true);
        var swWithMigration = new Stopwatch();
        swWithMigration.Start();
        
        IStartUpDocumentMigrationRunner documentMigrationRunner =
            TestcontainersContext.Provider.GetRequiredService<IStartUpDocumentMigrationRunner>();
        documentMigrationRunner.RunAll(client.GetDatabase(DatabaseName));
        swWithMigration.Stop();

        ClearCollection();

        var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

        TestContext.Out.WriteLine($"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {ToleranceMs}ms), Documents: {DocumentCount}, Migrations per Document: 2");
        
        // Assert
        result.Should().BeLessThan(ToleranceMs);
    }

    private static void InsertMany(int number, bool withVersion)
    {
        var documents = new List<BsonDocument>();
        for (var n = 0; n < number; n++)
        {
            var document = new BsonDocument
            {
                { "Dors", 3 }
            };
            if (withVersion)
            {
                document.Add("Version", "0.0.0");
            }

            documents.Add(document);
        }

        TestcontainersContext.MongoClient
            .GetDatabase(DatabaseName)
            .GetCollection<BsonDocument>(CollectionName)
            .InsertManyAsync(documents)
            .Wait();
    }

    private static void MigrateAll(bool withVersion)
    {
        IMongoClient client = TestcontainersContext.MongoClient;

        if (withVersion)
        {
            var versionedCollection = client.GetDatabase(DatabaseName)
                .GetCollection<TestDocumentWithTwoMigrationHighestVersion>(CollectionName);
            var versionedResult = versionedCollection.FindAsync(_ => true).Result.ToListAsync().Result;
            return;
        }
        var collection = client.GetDatabase(DatabaseName)
            .GetCollection<TestClass>(CollectionName);
        var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
    }

    private static void AddDocumentsToCache()
    {
        InsertMany(DocumentCount, false);
        MigrateAll(false);
    }

    private static void ClearCollection()
    {
        TestcontainersContext.MongoClient
            .GetDatabase(DatabaseName)
            .DropCollection(CollectionName);
    }
}