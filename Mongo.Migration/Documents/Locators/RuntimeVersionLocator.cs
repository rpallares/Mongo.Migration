using System.Collections.Frozen;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

internal class RuntimeVersionLocator : AbstractLocator<DocumentVersion, Type>, IRuntimeVersionLocator
{
    private readonly FrozenDictionary<Type, DocumentVersion> _codeDefinedDictionary;

    internal RuntimeVersionLocator(IEnumerable<KeyValuePair<Type, DocumentVersion>> alreadyDefinedRuntimeVersions)
    {
        _codeDefinedDictionary = alreadyDefinedRuntimeVersions.ToFrozenDictionary();
    }

    public override DocumentVersion? GetLocateOrNull(Type identifier)
    {
        if (_codeDefinedDictionary.TryGetValue(identifier, out DocumentVersion version))
        {
            return version;
        }

        if (LocatesDictionary.TryGetValue(identifier, out version))
        {
            return version;
        }

        return null;
    }

    public override void Locate()
    {
        LocatesDictionary = LocateAttributes<RuntimeVersionAttribute>()
            .ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2.Version);
    }
}