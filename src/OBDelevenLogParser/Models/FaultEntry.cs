namespace OBDelevenLogParser.Models;

public record FaultEntry(
    string Code,
    string Description,
    string Status,
    FaultSnapshot Snapshot);