using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Exceptions;

namespace Mongo.Migration.Startup.Static;

public static class MongoMigrationClient
{
    private static bool _isRunning;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        if (_isRunning)
        {
            throw new AlreadyInitializedException();
        }

        var app = serviceProvider.GetRequiredService<IMongoMigration>();
        app.Run();

        _isRunning = true;
    }

    public static void Reset()
    {
        _isRunning = false;
    }
}