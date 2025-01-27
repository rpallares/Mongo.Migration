using System.Collections.Frozen;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

internal class StartUpVersionLocator : AbstractLocator<DocumentVersion, Type>, IStartUpVersionLocator
{
    public override DocumentVersion? GetLocateOrNull(Type identifier)
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
        LocatesDictionary = LocateAttributes<StartUpVersionAttribute>()
           .ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2.Version);
    }
}