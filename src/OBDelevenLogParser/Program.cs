using System.Text.Json;
using OBDelevenLogParser;
using OBDelevenLogParser.Helpers;

const string defaultInput = "OBDeleven_Log.txt";

var arguments = CliHelpers.ParseArgs(args);
if (!arguments.IsValid)
    return CliHelpers.Fail(arguments.Error!);

var path = arguments.InputPath ?? defaultInput;
if (!File.Exists(path))
    return CliHelpers.Fail($"File not found: {path}");

var json = JsonSerializer.Serialize(
    OBDParser.Parse(File.ReadAllLines(path)),
    new JsonSerializerOptions { WriteIndented = true }
);

if (arguments.OutputPath is { } outputPath)
    FileHelpers.WriteToFile(outputPath, json);
else
    Console.WriteLine(json);

return 0;