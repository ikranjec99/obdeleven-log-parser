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
            EngineRpm:           f.Dbl("Engine speed"),
            VehicleSpeedKmh:     f.Dbl("Vehicle speed"),
            CoolantTempC:        f.Dbl("Coolant temperature"),
            IntakeAirTempC:      f.Dbl("Intake air temperature"),
            AmbientPressureMbar: f.Dbl("Ambient air pressure"),
            Voltage:             f.Dbl("Voltage terminal 30"),
            DynamicData:         f.Str("Dynamic environmental data"),
            Date:                f.TryGetValue("date", out var d) && DateTime.TryParse(d, out var dt) ? dt : null
        );
        
        private double Dbl(string k) => f.TryGetValue(k, out var v) && double.TryParse(v.Split(' ')[0], out var n) ? n : 0;
        private int Int(string k) => f.TryGetValue(k, out var v) && int.TryParse(v.Split(' ')[0], out var n) ? n : 0;
        private string Str(string k) => f.GetValueOrDefault(k, "");
    }
}