using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.Migrations.Database
{
    internal class DatabaseIntegrationTest : IDisposable
    {
        private const string MigrationsCollectionName = "_migrations";

        protected IMongoClient _client;

        protected IServiceProvider _serviceProvider;

        protected IMongoDatabase _db;

        protected MongoDbRunner _mongoToGoRunner;

        protected virtual string DatabaseName { get; set; } = "DatabaseMigration";

        protected virtual string CollectionName { get; set; } = "Test";

        public void Dispose()
        {
            _mongoToGoRunner?.Dispose();
        }

        protected virtual void OnSetUp(DocumentVersion databaseMigrationVersion)
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);
            _db = _client.GetDatabase(DatabaseName);
            _db.CreateCollection(CollectionName);


            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
                .AddSingleton<IMongoClient>(_client)
                .AddMigration(new MongoMigrationSettings
                {
                    ConnectionString = _mongoToGoRunner.ConnectionString,
                    Database = DatabaseName,
                    DatabaseMigrationVersion = databaseMigrationVersion
                });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected void InsertMigrations(IEnumerable<DatabaseMigration> migrations)
        {
            var list = migrations.Select(m => new BsonDocument { { "MigrationId", m.GetType().ToString() }, { "Version", m.Version.ToString() } });
            _db.GetCollection<BsonDocument>(MigrationsCollectionName).InsertManyAsync(list).Wait();
        }

        protected List<MigrationHistory> GetMigrationHistory()
        {
            var migrationHistoryCollection = _db.GetCollection<MigrationHistory>(MigrationsCollectionName);
            return migrationHistoryCollection.Find(m => true).ToList();
        }
    }
}