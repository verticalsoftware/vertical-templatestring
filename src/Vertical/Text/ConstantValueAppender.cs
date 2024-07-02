using System.Diagnostics.CodeAnalysis;

namespace Vertical.Text;

/// <summary>
/// Appends static values.
/// </summary>
internal sealed class ConstantValueAppender : IAppender
{
    private readonly string _value;

    internal ConstantValueAppender(string value)
    {
        _value = value;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"'{_value}'";

    /// <inheritdoc />
    public bool HasMappedProperties => false;

    /// <inheritdoc />
    public void Append(List<ContentToken> tokenList, IDictionary<string, object?> properties, bool optionalContext)
    {
        tokenList.Add(new ContentToken(TokenType.ConstantValue, _value));
    }
}