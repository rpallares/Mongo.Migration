using FluentAssertions;
using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents;

[TestFixture]
public class DocumentVersionWhenCreating
{
    [Test]
    public void If_Empty_Then_version_is_empty_value()
    {
        DocumentVersion version = DocumentVersion.Empty;

        version.ToString().Should().Be("-1.0.0");
    }

    [Test]
    public void If_Default_Then_version_is_default_value()
    {
        DocumentVersion version = DocumentVersion.Default;

        version.ToString().Should().Be("0.0.0");
    }

    [Test]
    public void If_first_part_contains_char_Then_exception_is_thrown()
    {
        Action act = () => DocumentVersion.Parse("a.0.0");

        act.Should().Throw<FormatException>();
    }

    [Test]
    public void If_new_version_with_int_Then_version_string_should_be_same()
    {
        var version = new DocumentVersion(1, 0, 2);

        version.ToString().Should().Be("1.0.2");
    }

    [Test]
    public void If_new_version_with_string_Then_version_string_should_be_same()
    {
        var version = DocumentVersion.Parse("1.0.2");

        version.ToString().Should().Be("1.0.2");
    }

    [Test]
    public void If_second_part_contains_char_Then_exception_is_thrown()
    {
        Action act = () => DocumentVersion.Parse("0.a.0");

        act.Should().Throw<FormatException>();
    }

    [Test]
    public void If_third_part_contains_char_Then_exception_is_thrown()
    {
        Action act = () => DocumentVersion.Parse("0.0.a");

        act.Should().Throw<FormatException>();
    }

    [Test]
    public void If_version_string_is_too_long_Then_exception_is_thrown()
    {
        Action act = () => DocumentVersion.Parse("0.0.0.0");

        act.Should().Throw<IndexOutOfRangeException>();
    }

    [Test]
    public void If_version_string_is_too_short1_then_default()
    {
        var version = DocumentVersion.Parse("33");

        version.ToString().Should().Be("33.0.0");
    }

    [Test]
    public void If_version_string_is_too_short2_then_default()
    {
        var version = DocumentVersion.Parse("42.27");

        version.ToString().Should().Be("42.27.0");
    }
}