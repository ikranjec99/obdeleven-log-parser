namespace OBDelevenLogParser.Models;

public record FaultSnapshot(
    int     Priority,
    int     FrequencyCounter,
    int     UnlearningCounter,
    int     MileageKm,
    double  EngineRpm,
    double  VehicleSpeedKmh,
    double  CoolantTempC,
    double  IntakeAirTempC,
    double  AmbientPressureMbar,
    double  Voltage,
    string  DynamicData,
    DateTime? Date);