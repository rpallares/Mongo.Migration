using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services.Interceptors;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services
{
    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;

        private readonly IMigrationInterceptorProvider _provider;

        private readonly DocumentVersionSerializer _serializer;

        private readonly IStartUpDatabaseMigrationRunner _startUpDatabaseMigrationRunner;

        private readonly IStartUpDocumentMigrationRunner _startUpDocumentMigrationRunner;

        public MigrationService(
            DocumentVersionSerializer serializer,
            IMigrationInterceptorProvider provider,
            ILogger<MigrationService> logger,
            IStartUpDocumentMigrationRunner startUpDocumentMigrationRunner,
            IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner)
        {
            _serializer = serializer;
            _provider = provider;
            _logger = logger;
            _startUpDocumentMigrationRunner = startUpDocumentMigrationRunner;
            _startUpDatabaseMigrationRunner = startUpDatabaseMigrationRunner;
        }

        public void Migrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
            RegisterSerializer();

            OnStartup();
        }

        private void OnStartup()
        {
            _startUpDatabaseMigrationRunner.RunAll();
            _startUpDocumentMigrationRunner.RunAll();
        }

        private void RegisterSerializer()
        {
            try
            {
                BsonSerializer.RegisterSerializer(_serializer.ValueType, _serializer);
            }
            catch (BsonSerializationException ex)
            {
                // Catch if Serializer was registered already ... not great, I know.
                // We have to do this, because there is always a default DocumentVersionSerialzer.
                // BsonSerializer.LookupSerializer(), does not work.

                _logger.LogError(ex, "Cannot register {Type} twice", typeof(DocumentVersionSerializer));
            }
        }
    }
}