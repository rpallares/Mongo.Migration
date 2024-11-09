using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Exceptions;

using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Documents;

public readonly struct DocumentVersion : IComparable<DocumentVersion>
{
    private const char VersionSplitChar = '.';

    private const int MaxLength = 3;

    public int Major { get; init; }

    public int Minor { get; init; }

    public int Revision { get; init; }

    static DocumentVersion()
    {
        try
        {
            BsonSerializer.RegisterSerializer(typeof(DocumentVersion), new DocumentVersionSerializer());
        }
        catch (Exception)
        {
        }
    }

    public DocumentVersion(string version)
    {
        string[] versionParts = version.Split(VersionSplitChar);

        if (versionParts.Length != MaxLength)
        {
            throw new VersionStringToLongException(version);
        }

        Major = ParseVersionPart(versionParts[0]);

        Minor = ParseVersionPart(versionParts[1]);

        Revision = ParseVersionPart(versionParts[2]);
    }

    public DocumentVersion(int major, int minor, int revision)
    {
        Major = major;
        Minor = minor;
        Revision = revision;
    }

    public static DocumentVersion Default()
    {
        return default(DocumentVersion);
    }

    public static DocumentVersion Empty()
    {
        return new(-1, 0, 0);
    }

    public static implicit operator DocumentVersion(string version)
    {
        return new(version);
    }

    public static implicit operator string(DocumentVersion documentVersion)
    {
        return documentVersion.ToString();
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Revision}";
    }

    public int CompareTo(DocumentVersion other)
    {
        if (Equals(other))
        {
            return 0;
        }

        return this > other ? 1 : -1;
    }

    public static bool operator ==(DocumentVersion a, DocumentVersion b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(DocumentVersion a, DocumentVersion b)
    {
        return !(a == b);
    }

    public static bool operator >(DocumentVersion a, DocumentVersion b)
    {
        return a.Major > b.Major
               || (a.Major == b.Major && a.Minor > b.Minor)
               || (a.Major == b.Major && a.Minor == b.Minor && a.Revision > b.Revision);
    }

    public static bool operator <(DocumentVersion a, DocumentVersion b)
    {
        return a != b && !(a > b);
    }

    public static bool operator <=(DocumentVersion a, DocumentVersion b)
    {
        return a == b || a < b;
    }

    public static bool operator >=(DocumentVersion a, DocumentVersion b)
    {
        return a == b || a > b;
    }

    public bool Equals(DocumentVersion other)
    {
        return other.Major == Major && other.Minor == Minor && other.Revision == Revision;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != typeof(DocumentVersion))
        {
            return false;
        }

        return Equals((DocumentVersion)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Revision);
    }

    private static int ParseVersionPart(string value)
    {
        if (!int.TryParse(value, out int target))
        {
            throw new InvalidVersionValueException(value);
        }

        return target;
    }
}