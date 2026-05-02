namespace OBDelevenLogParser.Models;

public record EcuModule(
    string Id,
    string SystemDescription,
    string SoftwareNumber,
    List<FaultEntry> Faults);