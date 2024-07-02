using System.Diagnostics.CodeAnalysis;

namespace Vertical.Text;

/// <summary>
/// Appends property values.
/// </summary>
internal sealed class PropertyValueAppender : IAppender
{
    private readonly string _propertyKey;
    private readonly string _templatePart;
    private readonly TemplateStringOptions _options;
    private readonly string? _format;

    internal PropertyValueAppender(
        string propertyKey,
        string? format,
        string templatePart,
        TemplateStringOptions options)
    {
        _propertyKey = propertyKey;
        _templatePart = templatePart;
        _options = options;
        _format = format != null ? $"{{0{format}}}" : null;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => _templatePart;

    /// <inheritdoc />
    public bool HasMappedProperties => true;

    /// <inheritdoc />
    public void Append(List<ContentToken> tokenList, IDictionary<string, object?> properties, bool optionalContext)
    {
        if (!properties.TryGetValue(_propertyKey, out var value))
        {
            TryThrowUnmatchedTemplateProperty();
            tokenList.Add(new ContentToken(TokenType.ConstantValue, _templatePart));
            return;
        }

        if (value != null)
        {
            var formattedValue = _format != null
                ? string.Format(_format, value)
                : value;
            tokenList.Add(new ContentToken(TokenType.PropertyValue, formattedValue));
            return;
        }

        TryThrowNonOptionalContext(optionalContext);
    }

    private void TryThrowNonOptionalContext(bool optionalContext)
    {
        if (optionalContext)
            return;

        throw new FormatException($"Property '{_propertyKey}' was null in the data properties, " + 
                                  "but not defined within an optional template.");
    }

    private void TryThrowUnmatchedTemplateProperty()
    {
        if (_options.PreserveUnmatchedTemplates)
            return;

        throw new FormatException($"Property '{_propertyKey}' was not provided in the data properties.");
    }
}