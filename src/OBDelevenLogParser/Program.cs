using System.Text.Json;
using OBDelevenLogParser;

var path = args.FirstOrDefault() ?? "OBDeleven_Log.txt";
if (!File.Exists(path)) { Console.Error.WriteLine($"File not found: {path}"); return 1; }

var log = OBDParser.Parse(File.ReadAllLines(path));
Console.WriteLine(JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true }));
return 0;