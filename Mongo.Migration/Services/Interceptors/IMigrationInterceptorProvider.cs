using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors;

public interface IMigrationInterceptorProvider : IRegistryAwareBsonSerializationProvider
{
    IMigrationInterceptorFactory MigrationInterceptorFactory { get; }
}