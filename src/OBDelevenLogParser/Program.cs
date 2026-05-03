using System.Text.Json;
using OBDelevenLogParser;
using OBDelevenLogParser.Helpers;

const string defaultInput = "OBDeleven_Log.txt";

ConsoleHelpers.WriteHeader();

var arguments = CliArgumentHelpers.ParseArgs(args);
if (!arguments.IsValid)
{
    ConsoleHelpers.WriteError(arguments.Error!);
    return 1;
}

var path = arguments.InputPath ?? defaultInput;
if (!File.Exists(path))
{
    ConsoleHelpers.WriteError($"File {path} does not exist");
    return 1;
}

var log = OBDParser.Parse(File.ReadAllLines(path));
ConsoleHelpers.WriteSummary(log, path, arguments.OutputPath);

var json = JsonSerializer.Serialize(
    log,
    new JsonSerializerOptions { WriteIndented = true }
);

if (arguments.OutputPath is { } outputPath)
    FileHelpers.WriteToFile(outputPath, json);
else
    Console.WriteLine(json);

return 0;
