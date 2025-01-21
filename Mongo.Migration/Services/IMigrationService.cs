namespace Mongo.Migration.Services;

public interface IMigrationService
{
    void RegisterBsonStatics();

    Task MigrateAsync(string databaseName, string? targetDatabaseVersion, CancellationToken cancellationToken);
}