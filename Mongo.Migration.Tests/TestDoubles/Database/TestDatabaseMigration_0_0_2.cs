using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.TestDoubles.Database;

internal class TestDatabaseMigration002 : DatabaseMigration
{
    public TestDatabaseMigration002()
        : base("0.0.2")
    {
    }

    public override void Up(IMongoDatabase db)
    {
    }

    public override void Down(IMongoDatabase db)
    {
    }
}