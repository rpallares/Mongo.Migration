using Mongo.Migration.Documents.Attributes;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Collections.Immutable;
#endif

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
#if NET8_0_OR_GREATER
            .ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2.CollectionInformation);
#else
            .ToImmutableDictionary(pair => pair.Item1, pair => pair.Item2.CollectionInformation);
#endif

    }

    public IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty()
    {
        return LocatesDictionary;
    }
}