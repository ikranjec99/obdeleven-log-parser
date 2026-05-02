namespace OBDelevenLogParser.Models;

public record VehicleInfo(
    string Vin,
    string Car,
    int Year,
    string Engine,
    int Mileage);