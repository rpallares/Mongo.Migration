using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo.Migration.Startup.Static;
using Mongo.Migration.Tests.TestDoubles;
using Mongo2Go;
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

    private MongoClient _client;

    private MongoDbRunner _runner;

    [TearDown]
    public void TearDown()
    {
        MongoMigrationClient.Reset();
        _client = null!;
        _runner.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _runner = MongoDbRunner.Start();
        _client = new(_runner.ConnectionString);
    }

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

        // Measure time of MongoDb processing without Mongo.Migration
        InsertMany(DocumentCount, true);
        var swWithMigration = new Stopwatch();
        swWithMigration.Start();
        ServiceCollection serviceCollection = new();
        serviceCollection
            .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(_client)
            .AddMigration(new MongoMigrationSettings());
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        MongoMigrationClient.Initialize(serviceProvider);
        swWithMigration.Stop();

        ClearCollection();

        var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

        Console.WriteLine(
            $"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {ToleranceMs}ms), Documents: {DocumentCount}, Migrations per Document: 2");

        // Assert
        result.Should().BeLessThan(ToleranceMs);
    }

    private void InsertMany(int number, bool withVersion)
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

        _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(CollectionName).InsertManyAsync(documents)
            .Wait();
    }

    private void MigrateAll(bool withVersion)
    {
        if (withVersion)
        {
            var versionedCollectin = _client.GetDatabase(DatabaseName)
                .GetCollection<TestDocumentWithTwoMigrationHighestVersion>(CollectionName);
            var versionedResult = versionedCollectin.FindAsync(_ => true).Result.ToListAsync().Result;
            return;
        }

        var collection = _client.GetDatabase(DatabaseName)
            .GetCollection<TestClass>(CollectionName);
        var result = collection.FindAsync(_ => true).Result.ToListAsync().Result;
    }

    private void AddDocumentsToCache()
    {
        InsertMany(DocumentCount, false);
        MigrateAll(false);
    }

    private void ClearCollection()
    {
        _client.GetDatabase(DatabaseName).DropCollection(CollectionName);
    }
}