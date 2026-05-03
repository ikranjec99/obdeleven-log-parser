namespace OBDelevenLogParser.Helpers;

public static class FileHelpers
{
    public static void WriteToFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, content);
    }
}