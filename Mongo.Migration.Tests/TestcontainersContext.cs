using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;

namespace Mongo.Migration.Tests;

[SetUpFixture]
public sealed class TestcontainersContext
{
    private static readonly MongoDbContainer s_mongoDbContainer =
        new MongoDbBuilder()
            .Build();
    private static MongoClient? s_mongoClient;

    public static IMongoClient MongoClient => s_mongoClient ?? throw new InvalidOperationException("Must be initialized");
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await s_mongoDbContainer.StartAsync();
        s_mongoClient = new MongoClient(s_mongoDbContainer.GetConnectionString());
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        s_mongoClient?.Dispose();
        s_mongoClient = null;
        await s_mongoDbContainer.StopAsync();
        await s_mongoDbContainer.DisposeAsync();
    }
}