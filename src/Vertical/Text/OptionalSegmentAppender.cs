using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Vertical.Text;

/// <summary>
/// Appends optional segments.
/// </summary>
internal sealed class OptionalSegmentAppender : IAppender
{
    private readonly string _templatePart;
    private readonly IReadOnlyCollection<IAppender> _appenders;

    internal OptionalSegmentAppender(string templatePart, IReadOnlyCollection<IAppender> appenders)
    {
        _templatePart = templatePart;
        _appenders = appenders;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool HasMappedProperties => _appenders.Any(appender => appender.HasMappedProperties);

    /// <inheritdoc />
    public void Append(List<ContentToken> tokenList, IDictionary<string, object?> properties, bool optionalContext)
    {
        var subTokens = _appenders.Aggregate(new List<ContentToken>(16),
            (list, appender) =>
            {
                appender.Append(list, properties, true);
                return list;
            });

        if (subTokens.All(token => token.Type == TokenType.ConstantValue))
            return;
        
        tokenList.AddRange(subTokens);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => _templatePart;
}