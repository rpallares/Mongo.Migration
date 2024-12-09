﻿namespace Mongo.Migration.Documents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StartUpVersion : Attribute
{
    public DocumentVersion Version { get; }

    public StartUpVersion(string version)
    {
        Version = DocumentVersion.Parse(version.AsSpan());
    }
}