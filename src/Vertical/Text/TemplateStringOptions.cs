namespace Vertical.Text;

/// <summary>
/// Defines behavior when handlebar strings are created.
/// </summary>
public sealed class TemplateStringOptions
{
    /// <summary>
    /// Gets/sets whether to preserve templates when they are not matched with properties.
    /// </summary>
    public bool PreserveUnmatchedTemplates { get; init; }
}