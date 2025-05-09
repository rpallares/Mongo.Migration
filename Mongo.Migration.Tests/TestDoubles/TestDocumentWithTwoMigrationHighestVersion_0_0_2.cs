﻿using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

public class TestDocumentWithTwoMigrationHighestVersion002 : DocumentMigration<TestDocumentWithTwoMigrationHighestVersion>
{
    public TestDocumentWithTwoMigrationHighestVersion002()
        : base("0.0.2")
    {
    }

    public override void Up(BsonDocument document)
    {
        var doors = document["Doors"].ToInt32();
        document.Add("Door", doors);
        document.Remove("Doors");
    }

    public override void Down(BsonDocument document)
    {
        var doors = document["Door"].ToInt32();
        document.Add("Doors", doors);
        document.Remove("Door");
    }
}