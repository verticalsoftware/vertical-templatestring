using System.Text;

namespace Vertical.Text;

/// <summary>
/// Represents a string that contains property placeholders ("holes") that can be substituted
/// for real values.
/// </summary>
public sealed class TemplateString
{
    private readonly IReadOnlyCollection<IAppender> _appenders;

    private TemplateString(IReadOnlyCollection<IAppender> appenders)
    {
        _appenders = appenders;
    }

    /// <summary>
    /// Creates a string composed by substituting the template string with properties
    /// found in the given dictionary.
    /// </summary>
    /// <param name="properties">A dictionary of values used to populate the return value.</param>
    /// <returns><see cref="string"/></returns>
    public string Replace(IDictionary<string, object?> properties)
    {
        var tokens = _appenders.Aggregate(new List<ContentToken>(16), (tokens, appender) =>
        {
            appender.Append(tokens, properties, optionalContext: false);
            return tokens;
        });

        var sb = tokens.Aggregate(new StringBuilder(250), (sb, token) =>
        {
            sb.Append(token.Value);
            return sb;
        });

        return sb.ToString();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateString"/> class.
    /// </summary>
    /// <param name="template">Template string that defines format and placement of property symbols.</param>
    /// <param name="options">Options that control replacement behavior.</param>
    /// <returns><see cref="TemplateString"/></returns>
    /// <exception cref="ArgumentException">The template is null or whitespace.</exception>
    /// <exception cref="FormatException">The template is invalid.</exception>
    public static TemplateString Create(string template, TemplateStringOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            throw new ArgumentException("Provided template string was null or whitespace.");
        }

        var parser = new TemplateParser(template, options ?? new TemplateStringOptions());
        return new TemplateString(parser.Build());
    }
}