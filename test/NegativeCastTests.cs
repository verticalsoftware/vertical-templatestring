namespace Vertical.Text.Tests;

public class NegativeCastTests
{
    [Theory, MemberData(nameof(InvalidTemplateTheories))]
    public void Throws_For_Invalid_Template(string template)
    {
        Assert.Throws<FormatException>(() => TemplateString.Create(template));
    }

    [Fact]
    public void Throws_For_Unmatched_Template()
    {
        var unit = TemplateString.Create("{property}");
        Assert.Throws<FormatException>(() => unit.Replace(new Dictionary<string, object?>()));
    }

    [Fact]
    public void Throws_For_Non_Optional_Null_Property()
    {
        var unit = TemplateString.Create("{property}");
        Assert.Throws<FormatException>(() => unit.Replace(new Dictionary<string, object?> { ["property"] = null }));
    }

    [Theory, InlineData((string?)null), InlineData(""), InlineData("  ")]
    public void Throws_For_NullWhiteSpace_Template(string? str)
    {
        Assert.Throws<ArgumentException>(() => TemplateString.Create(str!));
    }
    
    public static TheoryData<string> InvalidTemplateTheories => new()
    {
        "{unclosed",
        "[unclosed",
        "unopened}",
        "unopened]"
    };
}