using System.Globalization;
using OBDelevenLogParser.Models;

namespace OBDelevenLogParser.Extensions;

public static class DictionaryExtensions
{
    extension(Dictionary<string, string> f)
    {
        public FaultSnapshot ToSnapshot() => new(
            Priority:            f.Int("Priority"),
            FrequencyCounter:    f.Int("Malfunction frequency counter"),
            UnlearningCounter:   f.Int("Unlearning counter"),
            MileageKm:           f.Int("km-Mileage"),
            EngineRpm:           f.Double("Engine speed"),
            VehicleSpeedKmh:     f.Double("Vehicle speed"),
            CoolantTempC:        f.Double("Coolant temperature"),
            IntakeAirTempC:      f.Double("Intake air temperature"),
            AmbientPressureMbar: f.Double("Ambient air pressure"),
            Voltage:             f.Double("Voltage terminal 30"),
            DynamicData:         f.String("Dynamic environmental data"),
            Date:                f.TryGetValue("date", out var d) && DateTime.TryParse(d, out var dt) ? dt : null
        );
        
        private double Double(string k) => 
            f.TryGetValue(k, out var v) 
            && double.TryParse(v.Split(' ')[0], CultureInfo.InvariantCulture, out var n) ? n : 0;
        
        private int Int(string k) => 
            f.TryGetValue(k, out var v) 
            && int.TryParse(v.Split(' ')[0], CultureInfo.InvariantCulture ,out var n) ? n : 0;
        
        private string String(string k) => f.GetValueOrDefault(k, "");
    }
}