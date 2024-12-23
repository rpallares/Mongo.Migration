namespace Mongo.Migration.Exceptions;

internal class DuplicateVersionException : Exception
{
    public DuplicateVersionException(string typeName, string version)
        : base($"Migration '{typeName}' contains duplicate version: {version}")
    {
    }
}