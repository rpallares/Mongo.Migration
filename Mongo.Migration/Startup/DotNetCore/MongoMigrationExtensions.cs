using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services;
namespace Mongo.Migration.Startup.DotNetCore;

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

    public static async Task InitializeAndMigrateAsync(this IApplicationBuilder app, string databaseName, string? targetDatabaseVersion)
    {
        IMigrationService migrationService = app.ApplicationServices.GetRequiredService<IMigrationService>();
        migrationService.RegisterBsonStatics();

        await migrationService.MigrateAsync(databaseName, targetDatabaseVersion);
    }
}