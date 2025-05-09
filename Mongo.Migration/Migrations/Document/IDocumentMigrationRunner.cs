﻿using Mongo.Migration.Documents;

using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document;

public interface IDocumentMigrationRunner
{
    void Run(Type type, BsonDocument document, in DocumentVersion to);

    void Run(Type type, BsonDocument document);
}