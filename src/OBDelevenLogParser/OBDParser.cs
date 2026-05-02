using OBDelevenLogParser.Extensions;
using OBDelevenLogParser.Helpers;
using OBDelevenLogParser.Models;

namespace OBDelevenLogParser;

public static class OBDParser
{
    public static OBDLog Parse(string[] lines)
    {
        var vehicle= ParseVehicle(lines);
        var modules = ParseModules(lines);
        return new OBDLog(vehicle.LogDate, vehicle.Info, modules);
    }
    
    private static bool IsDtcLine(string line)
    {
        var idx = line.IndexOf(" - ", StringComparison.Ordinal);
        if (idx < 5) return false;
        var code = line[..idx];
        return code.Length >= 6 && char.IsLetter(code[0]) && code[1..].All(char.IsAsciiHexDigit);
    }

    private static List<FaultEntry> ParseFaults(string[] lines)
    {
        var faults = new List<FaultEntry>();
        foreach (var block in SplitOn(lines, IsDtcLine))
        {
            var dtc = block[0];
            var dash= dtc.IndexOf(" - ", StringComparison.Ordinal);
            var code   = dtc[..dash];
            var desc   = dtc[(dash + 3)..];
            var status = block.Skip(1).FirstOrDefault(l => !l.Contains(" - ")) ?? "";

            // Collect all key-value lines; last value wins on duplicate keys (e.g. Priority)
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var l in block.Skip(1).Where(l => l.Contains(" - ") && !IsDtcLine(l)))
            {
                var parts = l.Split(" - ", 2);
                fields[parts[0].Trim()] = parts[1].Trim();
            }

            faults.Add(new FaultEntry(code, desc, status, fields.ToSnapshot()));
        }
        return faults;
    }
    
    private static List<EcuModule> ParseModules(string[] lines)
    {
        var modules = new List<EcuModule>();
        foreach (var block in SplitOn(lines, ParserHelpers.IsEcuHeader))
        {
            var trimmed = block.Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
            var meta    = ParserHelpers.TakeUntil(trimmed, l => l == "Faults:");
            var faults  = ParseFaults(trimmed.Skip(meta.Length + 1).ToArray());

            modules.Add(new EcuModule(
                Id:                trimmed[0],
                SystemDescription: ParserHelpers.MetaField(meta, "System description:") ?? trimmed[0],
                SoftwareNumber:    ParserHelpers.MetaField(meta, "Software number:")    ?? "",
                Faults:            faults
            ));
        }
        return modules;
    }
    
    private static (DateTime LogDate, VehicleInfo Info) ParseVehicle(string[] lines)
    {
        var h = lines.Take(15).Select(l => l.Trim()).ToArray();
        return (
            LogDate: ParserHelpers.Field<DateTime>(h, "Date:", DateTime.Parse),
            Info: new VehicleInfo(
                Vin:     ParserHelpers.Field(h, "VIN:"),
                Car:     ParserHelpers.Field(h, "Car:"),
                Year:    ParserHelpers.Field<int>(h, "Year:", int.Parse),
                Engine:  ParserHelpers.Field(h, "Engine:"),
                Mileage: ParserHelpers.Field<int>(h, "Mileage:", s => int.Parse(new string(s.TakeWhile(char.IsDigit).ToArray())))
            )
        );
    }
    
    private static IEnumerable<string[]> SplitOn(string[] lines, Func<string, bool> isHeader)
    {
        var block = new List<string>();
        foreach (var line in lines)
        {
            if (isHeader(line) && block.Count > 0) { yield return [.. block]; block.Clear(); }
            if (isHeader(line) || block.Count > 0) block.Add(line);
        }
        if (block.Count > 0) yield return [.. block];
    }
}