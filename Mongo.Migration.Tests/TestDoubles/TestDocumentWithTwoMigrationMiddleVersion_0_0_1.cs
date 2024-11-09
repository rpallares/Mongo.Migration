﻿using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

internal class TestDocumentWithTwoMigrationMiddleVersion001 : DocumentMigration<TestDocumentWithTwoMigrationMiddleVersion>
{
    public TestDocumentWithTwoMigrationMiddleVersion001()
        : base("0.0.1")
    {
    }

    public override void Up(BsonDocument document)
    {
        var doors = document["Dors"].ToInt32();
        document.Add("Doors", doors);
        document.Remove("Dors");
    }

    public override void Down(BsonDocument document)
    {
        var doors = document["Doors"].ToInt32();
        document.Add("Dors", doors);
        document.Remove("Doors");
    }
}