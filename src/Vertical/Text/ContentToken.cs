namespace Vertical.Text;

/// <summary>
/// Represents a content token that is added to an output string.
/// </summary>
/// <param name="type">Token type</param>
/// <param name="value">Literal value</param>
public readonly struct ContentToken(TokenType type, object value)
{
    /// <summary>
    /// Gets the token type
    /// </summary>
    public TokenType Type { get; } = type;

    /// <summary>
    /// Gets the content to add to the string
    /// </summary>
    public object Value { get; } = value;
}