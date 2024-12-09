namespace Mongo.Migration.Startup.DotNetCore;

internal class MongoMigrationStartupSettings
{
    public bool RuntimeMigrationEnabled { get; set; }
    public bool StartupDocumentMigrationEnabled { get; set; }
    public bool DatabaseMigrationEnabled { get; set; }
}