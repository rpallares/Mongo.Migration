using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests;

[TestFixture]
public class IntegrationTest
{
    private const string DatabaseName = "PerformanceTest";
    private const string CollectionName = "Test";

    [SetUp]
    protected async Task SetUpAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        await client.GetDatabase(DatabaseName).CreateCollectionAsync(CollectionName);
    }

    [TearDown]
    protected async Task TearDownAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        await client.GetDatabase(DatabaseName)
            .DropCollectionAsync(CollectionName);
    }
}