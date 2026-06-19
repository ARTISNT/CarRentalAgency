using System.Globalization;
using System.Text.RegularExpressions;
using ContractService.Application.Abstractions.Services;

namespace ContractService.Application.Services;

public partial class TemplateRenderer : ITemplateRenderer
{
    private static readonly Regex PlaceholderRegex = PlaceholderRegexBuilder();

    [GeneratedRegex(@"\{\{\s*(?<path>[A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*)\s*\}\}")]
    private static partial Regex PlaceholderRegexBuilder();

    public string Render(string template, IReadOnlyDictionary<string, object?> variables)
    {
        if (string.IsNullOrEmpty(template))
            return template ?? string.Empty;

        return PlaceholderRegex.Replace(template, match =>
        {
            var path = match.Groups["path"].Value;
            return ResolvePath(variables, path) ?? string.Empty;
        });
    }

    private static string? ResolvePath(IReadOnlyDictionary<string, object?> variables, string path)
    {
        var segments = path.Split('.');
        if (segments.Length == 0)
            return null;

        if (!variables.TryGetValue(segments[0], out var current) || current is null)
            return null;

        for (var i = 1; i < segments.Length; i++)
        {
            if (current is null)
                return null;

            current = GetPropertyValue(current, segments[i]);
        }

        return FormatValue(current);
    }

    private static object? GetPropertyValue(object source, string name)
    {
        if (source is IDictionary<string, object?> dict)
        {
            return dict.TryGetValue(name, out var v) ? v : null;
        }

        var prop = source.GetType().GetProperty(name);
        return prop?.GetValue(source);
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            string s => s,
            DateTime dt => dt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            DateOnly d => d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            bool b => b ? "Да" : "Нет",
            decimal dec => dec.ToString("0.##", CultureInfo.InvariantCulture),
            double dbl => dbl.ToString("0.##", CultureInfo.InvariantCulture),
            float fl => fl.ToString("0.##", CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            long l => l.ToString(CultureInfo.InvariantCulture),
            _ => value.ToString(),
        };
    }
}
