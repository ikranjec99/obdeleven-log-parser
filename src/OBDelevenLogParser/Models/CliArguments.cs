namespace OBDelevenLogParser.Models;

public record CliArguments
{
    public string? Error { get; init; }
    public string? InputPath { get; init; }
    public string? OutputPath { get; init; }

    public bool IsValid => Error is null;

    public static CliArguments Valid(string? inputPath, string? outputPath) =>
        new() { InputPath = inputPath, OutputPath = outputPath };

    public static CliArguments Failure(string error) =>
        new() { Error = error };
}