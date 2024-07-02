namespace Vertical.Text;

internal sealed class TemplateParser
{
    private readonly string _template;
    private readonly TemplateStringOptions _options;
    private readonly Stack<char> _stack = new();

    private readonly ref struct TokenSpan(ReadOnlySpan<char> template, ReadOnlySpan<char> inner)
    {
        public ReadOnlySpan<char> Template { get; } = template;
        public ReadOnlySpan<char> Inner { get; } = inner;
        public int Length => Math.Max(Template.Length, Inner.Length); 
        public bool IsTemplate => Template.Length > 0;

        public bool IsEscapedTemplate => Template.Length >= 4 && Template is ['{', '{', .., '}', '}'];
    }
    
    internal TemplateParser(string template, TemplateStringOptions options)
    {
        _template = template;
        _options = options;
    }

    internal IReadOnlyCollection<IAppender> Build()
    {
        return Parse(_template.AsSpan(), new List<IAppender>(16));
    }

    private IReadOnlyCollection<IAppender> Parse(ReadOnlySpan<char> span, List<IAppender> appenders)
    {
        while (span.Length > 0)
        {
            span = ReadSpan(span, appenders);
        }

        return appenders;
    }

    private ReadOnlySpan<char> ReadSpan(ReadOnlySpan<char> span, List<IAppender> appenders)
    {
        var tokenSpan = ReadToken(span);

        return tokenSpan.IsTemplate
            ? AddTemplateAppender(span, tokenSpan, appenders)
            : AddConstantAppender(span, tokenSpan, appenders);
    }

    private ReadOnlySpan<char> AddTemplateAppender(
        ReadOnlySpan<char> span, 
        TokenSpan tokenSpan, 
        List<IAppender> appenders)
    {
        if (!tokenSpan.IsEscapedTemplate)
        {
            return span[0] == '{'
                ? AddPropertyValueAppender(span, tokenSpan, appenders)
                : AddOptionalSegmentAppender(span, tokenSpan, appenders);
        }

        appenders.Add(new ConstantValueAppender(new string(tokenSpan.Template[1..^1])));
        return span[tokenSpan.Length..];
    }

    private ReadOnlySpan<char> AddOptionalSegmentAppender(
        ReadOnlySpan<char> span, 
        TokenSpan tokenSpan, 
        List<IAppender> appenders)
    {
        var subAppenders = new List<IAppender>(6);
        Parse(tokenSpan.Inner, subAppenders);
        IAppender appender = subAppenders.Any(sub => sub.HasMappedProperties)
            ? new OptionalSegmentAppender(new string(tokenSpan.Template), subAppenders.ToArray())
            : new ConstantValueAppender(new string(tokenSpan.Template));
        
        appenders.Add(appender);

        return span[tokenSpan.Length..];
    }

    private ReadOnlySpan<char> AddPropertyValueAppender(
        ReadOnlySpan<char> span, 
        TokenSpan tokenSpan, 
        List<IAppender> appenders)
    {
        var colonIndex = -1;
        var inner = tokenSpan.Inner;

        for (var c = 0; c < inner.Length; c++)
        {
            if (inner[c] != ':') 
                continue;

            colonIndex = c;
            break;
        }

        var propertyKey = colonIndex == -1
            ? new string(tokenSpan.Inner)
            : new string(tokenSpan.Inner[..colonIndex]);
        
        var format = colonIndex == -1
            ? null
            : new string(tokenSpan.Inner[colonIndex..]);
        
        appenders.Add(new PropertyValueAppender(propertyKey, format, new string(tokenSpan.Template), _options));
        return span[tokenSpan.Length..];
    }

    private static ReadOnlySpan<char> AddConstantAppender(
        ReadOnlySpan<char> span, 
        TokenSpan tokenSpan, 
        List<IAppender> appenders)
    {
        appenders.Add(new ConstantValueAppender(new string(tokenSpan.Inner)));
        return span[tokenSpan.Inner.Length..];
    }

    private TokenSpan ReadToken(ReadOnlySpan<char> span)
    {
        return TryReadTemplateSpan(span, out var templateSpan) 
            ? templateSpan 
            : ReadValueSpan(span);
    }

    private bool TryReadTemplateSpan(ReadOnlySpan<char> span, out TokenSpan tokenSpan)
    {
        tokenSpan = default;

        _stack.Clear();

        for (var c = 0; c < span.Length; c++)
        {
            var chr = span[c];

            switch (chr)
            {
                case '[':
                    _stack.Push(']');
                    break;
                
                case '{' when c < span.Length - 1 && span[c+1] == '{':
                    for (var d = span.Length - 1; d > c; d--)
                    {
                        if (span[d] != '}' || span[d - 1] != '}')
                            continue;
                        tokenSpan = new(span[c..(d + 1)], span[(c+1)..d]);
                        return true;
                    }

                    throw GetFormatException("expected closing to escaped braces.");
                
                case '{':
                    _stack.Push('}');
                    break;
                
                case ']':
                case '}':
                    if (!_stack.TryPop(out var top) || chr != top)
                    {
                        throw GetFormatException($"invalid closing template character '{chr}'");
                    }

                    if (_stack.Any())
                        break;

                    var inner = span[1..c];
                    tokenSpan = new TokenSpan(span[..++c], inner);
                    return true;
                
                case { } when c == 0:
                    return false;
            }
        }

        throw GetFormatException($"expected '{_stack.Pop()}' but reached end of template");
    }

    private static TokenSpan ReadValueSpan(ReadOnlySpan<char> span)
    {
        for (var c = 0; c < span.Length; c++)
        {
            switch (span[c])
            {
                case '[':
                case ']':
                case '{':
                case '}':
                    return new TokenSpan([], span[..c]);
            }
        }

        return new TokenSpan([], span);
    }

    private FormatException GetFormatException(string message) => new(
        $"Input string '{_template}' was not in the correct format: {message}");
}