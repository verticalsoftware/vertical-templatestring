namespace Vertical.Text;

/// <summary>
/// Performs string append operations.
/// </summary>
internal interface IAppender
{
    /// <summary>
    /// Gets whether the appender has at least one mapped property.
    /// </summary>
    bool HasMappedProperties { get; }

    /// <summary>
    /// Appends content to the string builder.
    /// </summary>
    /// <param name="tokenList">List or tokens to add to the final output string.</param>
    /// <param name="properties">Properties being replaced in the template string.</param>
    /// <param name="optionalContext">Gets whether the operation is in an optional context.</param>
    void Append(List<ContentToken> tokenList, 
        IDictionary<string, object?> properties,
        bool optionalContext);
}