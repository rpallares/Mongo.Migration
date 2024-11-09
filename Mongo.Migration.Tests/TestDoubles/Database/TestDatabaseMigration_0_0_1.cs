using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.TestDoubles.Database;

internal class TestDatabaseMigration001 : DatabaseMigration
{
    public TestDatabaseMigration001()
        : base("0.0.1")
    {
    }

    public override void Up(IMongoDatabase db)
    {
    }

    public override void Down(IMongoDatabase db)
    {
    }
}