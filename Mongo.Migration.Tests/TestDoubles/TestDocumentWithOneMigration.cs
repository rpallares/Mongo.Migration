﻿using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Tests.TestDoubles;

[RuntimeVersion("0.0.1")]
internal class TestDocumentWithOneMigration : Document
{
    public int Doors { get; set; }
}

internal class TestDocumentWithoutAttribute : Document
{
    public int Doors { get; set; }
}