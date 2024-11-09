using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.TestDoubles.Database;

internal class TestDatabaseMigration003 : DatabaseMigration
{
    public TestDatabaseMigration003()
        : base("0.0.3")
    {
    }

    public override void Up(IMongoDatabase db)
    {
    }

    public override void Down(IMongoDatabase db)
    {
    }
}