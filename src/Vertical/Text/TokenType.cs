namespace Vertical.Text;

/// <summary>
/// Represents a token type.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Indicates the value was a string constant.
    /// </summary>
    ConstantValue,
    
    /// <summary>
    /// Indicates the value was a property replacement.
    /// </summary>
    PropertyValue
}