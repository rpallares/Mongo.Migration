using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services;
namespace Mongo.Migration.Startup;

public static class MongoMigrationExtensions
{
    public static IServiceCollection AddMigration(
        this IServiceCollection services,
        Action<MongoMigrationBuilder>? configure = null)
    {
        MongoMigrationBuilder builder = new MongoMigrationBuilder(services);

        configure?.Invoke(builder);

        builder.AddAllMigrationsIfNothingWasAdded();

        return services;
    }

    public static async Task InitializeAndMigrateAsync(this IMigrationService migrationService, string databaseName, string? targetDatabaseVersion)
    {
        migrationService.RegisterBsonStatics();

        await migrationService.MigrateAsync(databaseName, targetDatabaseVersion);
    }
}