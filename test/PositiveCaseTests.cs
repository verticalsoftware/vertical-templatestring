namespace Vertical.Text.Tests;

public class PositiveCaseTests
{
    private static readonly TemplateStringOptions Options = new()
    {
        PreserveUnmatchedTemplates = true
    };
    
    [Theory, MemberData(nameof(Theories))]
    public void Replace_Returns_Expected(string template, (string, object?)[] properties, string expected)
    {
        var unit = TemplateString.Create(template, Options);
        var dictionary = properties.ToDictionary(i => i.Item1, i => i.Item2);
        var actual = unit.Replace(dictionary);
        
        Assert.Equal(expected, actual);
    }
        
    public static TheoryData<string, (string,object?)[], string> Theories = new()
    {
        // No templates
        { "plain", [], "plain" },
        
        // Single template
        { "{property}", [("property", "value")], "value" },
        
        // Multiple templates
        { "{size}_{color}", [("color", "red"),("size", "big")], "big_red"},
        
        // Formatted templates
        { "{count:D4}", [("count", 5)], "0005"},
        
        // Unmatched template before
        { "{size}_{color}", [("color", "red")], "{size}_red" },
        
        // Unmatched template after
        { "{size}_{color}", [("size", "big")], "big_{color}" },
        
        // Optional w/value
        { ".json[.{compression}]", [("compression", "gz")], ".json.gz" },
        
        // Optional no value
        { ".json[.{compression}]", [("compression", null)], ".json" },
        
        // Nested optional, no values
        { "[.{format}[.{compression}]]", [("format", null), ("compression", null)], ""},
        
        // Nested optional, w/first value
        { "[.{format}[.{compression}]]", [("format", "json"), ("compression", null)], ".json"},
        
        // Nested optional, w/last value
        { "[.{format}[.{compression}]]", [("format", null), ("compression", "gz")], "..gz"},
        
        // Nested optional, both values
        { "[.{format}[.{compression}]]", [("format", "json"), ("compression", "gz")], ".json.gz"},
        
        // Escaped
        { "{{escaped}}", [], "{escaped}" },
        
        // Escaped mixed
        { "{red}{{green}}{blue}", [("red", "red"),("blue","blue")], "red{green}blue" },
        
        // Escaped at end
        { "{red}{green}{{blue}}", [("red", "red"),("green","green")], "redgreen{blue}" }
    };
}