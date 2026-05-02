namespace OBDelevenLogParser.Helpers;

public static class ParserHelpers
{
    public static string Field(string[] lines, string prefix) => MetaField(lines, prefix) ?? "";
    
    public static T Field<T>(string[] lines, string prefix, Func<string, T> parse) =>
        MetaField(lines, prefix) is { } v ? parse(v) : default!;
    
    public static bool IsEcuHeader(string line) =>
        line.Length > 3 && char.IsDigit(line[0]) && char.IsDigit(line[1]) && line[2] == ' ';
    
    public static string? MetaField(string[] lines, string prefix) =>
        lines.Select(l => l.Trim())
            .Where(l => l.StartsWith(prefix))
            .Select(l => l[prefix.Length..].Trim())
            .FirstOrDefault();
    
    public static string[] TakeUntil(string[] lines, Func<string, bool> stop) =>
        lines.TakeWhile(l => !stop(l)).ToArray();
}