﻿using MongoDB.Bson;

namespace Mongo.Migration.Tests.TestDoubles;

public class TestClass
{
    public ObjectId Id { get; set; }

    public int Dors { get; set; }

    public required string Version { get; set; }
}