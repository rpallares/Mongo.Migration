using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal class DatabaseIntegrationTest
{
    private const string MigrationsCollectionName = "_migrations";

    private ServiceProvider? _serviceProvider;
    protected IServiceProvider Provider => _serviceProvider ?? throw new InvalidOperationException("Must be setup");

    private IMongoDatabase? _db;
    protected IMongoDatabase Db => _db ?? throw new InvalidOperationException("Must be setup");

    protected virtual string DatabaseName { get; set; } = "DatabaseMigration";

    protected virtual string CollectionName { get; set; } = "Test";
    
    protected async Task OnSetUpAsync(DocumentVersion databaseMigrationVersion)
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        _db = client.GetDatabase(DatabaseName);
        await _db.CreateCollectionAsync(CollectionName);

        ServiceCollection serviceCollection = new();
        serviceCollection
            .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(client)
            .AddMigration(new MongoMigrationSettings
            {
                Database = DatabaseName,
                DatabaseMigrationVersion = databaseMigrationVersion
            });

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }
        IMongoClient client = TestcontainersContext.MongoClient;
        IMongoDatabase database = client.GetDatabase(DatabaseName);
        await database.DropCollectionAsync(CollectionName);
        await database.DropCollectionAsync(MigrationsCollectionName);
    }

    protected void InsertMigrations(IEnumerable<DatabaseMigration> migrations)
    {
        var list = migrations.Select(m => new BsonDocument { { "MigrationId", m.GetType().ToString() }, { "Version", m.Version.ToString() } });
        Db.GetCollection<BsonDocument>(MigrationsCollectionName).InsertManyAsync(list).Wait();
    }

    protected List<MigrationHistory> GetMigrationHistory()
    {
        var migrationHistoryCollection = Db.GetCollection<MigrationHistory>(MigrationsCollectionName);
        return migrationHistoryCollection.Find(m => true).ToList();
    }
}