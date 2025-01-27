using Mongo.Migration.Documents.Attributes;
using System.Collections.Frozen;

namespace Mongo.Migration.Documents.Locators;

public class CollectionLocator : AbstractLocator<CollectionLocationInformation, Type>, ICollectionLocator
{
    public override CollectionLocationInformation? GetLocateOrNull(Type identifier)
    {
        if (!LocatesDictionary.ContainsKey(identifier))
        {
            return null;
        }

        LocatesDictionary.TryGetValue(identifier, out var value);
        return value;
    }

    public override void Locate()
    {
        LocatesDictionary = LocateAttributes<CollectionLocationAttribute>()
            .ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2.CollectionInformation);
    }

    public IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty()
    {
        return LocatesDictionary;
    }
}