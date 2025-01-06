namespace Mongo.Migration.Documents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CollectionLocation : Attribute
{
    public CollectionLocationInformation CollectionInformation { get; }

    public CollectionLocation(string collectionName)
    {
        CollectionInformation = new CollectionLocationInformation(collectionName);
    }
}