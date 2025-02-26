#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Collections.Immutable;
#endif
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

internal class RuntimeVersionLocator : AbstractLocator<DocumentVersion, Type>, IRuntimeVersionLocator
{
#if NET8_0_OR_GREATER
    private readonly FrozenDictionary<Type, DocumentVersion> _codeDefinedDictionary;
#else
    private readonly ImmutableDictionary<Type, DocumentVersion> _codeDefinedDictionary;
#endif

    internal RuntimeVersionLocator(IEnumerable<KeyValuePair<Type, DocumentVersion>> alreadyDefinedRuntimeVersions)
    {
#if NET8_0_OR_GREATER
        _codeDefinedDictionary = alreadyDefinedRuntimeVersions.ToFrozenDictionary();
#else
        _codeDefinedDictionary = alreadyDefinedRuntimeVersions.ToImmutableDictionary();
#endif
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
#if NET8_0_OR_GREATER
            .ToFrozenDictionary(pair => pair.Item1, pair => pair.Item2.Version);
#else
            .ToImmutableDictionary(pair => pair.Item1, pair => pair.Item2.Version);
#endif
    }
}