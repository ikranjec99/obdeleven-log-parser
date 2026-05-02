namespace OBDelevenLogParser.Models;

public record OBDLog(
    DateTime LogDate,
    VehicleInfo Vehicle,
    List<EcuModule> Modules);