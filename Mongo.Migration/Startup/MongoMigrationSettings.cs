using Mongo.Migration.Documents;

namespace Mongo.Migration.Startup;

public class MongoMigrationSettings
{
    public string VersionFieldName { get; set; } = "Version";
}