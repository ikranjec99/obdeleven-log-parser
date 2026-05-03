using OBDelevenLogParser.Models;

namespace OBDelevenLogParser.Helpers;

public static class CliArgumentHelpers
{
    public static CliArguments ParseArgs(string[] args)
    {
        string? inputPath  = null;
        string? outputPath = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg is "-i" or "--input")
            {
                if (!TryNextArg(args, ref i, out var val))
                    return CliArguments.Failure($"Missing input path after {arg}.");
                inputPath = val;
            }
            else if (arg.StartsWith("--input=", StringComparison.Ordinal))
            {
                var val = arg["--input=".Length..];
                if (string.IsNullOrWhiteSpace(val))
                    return CliArguments.Failure("Missing input path after --input=.");
                inputPath = val;
            }
            else if (arg is "-o" or "--output")
            {
                if (!TryNextArg(args, ref i, out var val))
                    return CliArguments.Failure($"Missing output path after {arg}.");
                outputPath = val;
            }
            else if (arg.StartsWith("--output=", StringComparison.Ordinal))
            {
                var val = arg["--output=".Length..];
                if (string.IsNullOrWhiteSpace(val))
                    return CliArguments.Failure("Missing output path after --output=.");
                outputPath = val;
            }
            else if (arg.StartsWith('-'))
                return CliArguments.Failure($"Unknown option: {arg}");
            else if (inputPath is not null)
                return CliArguments.Failure($"Unexpected argument: {arg}");
            else
                inputPath = arg;
        }

        return CliArguments.Valid(inputPath, outputPath);
    }
    
    static bool TryNextArg(string[] args, ref int i, out string value)
    {
        value = ++i < args.Length ? args[i] : "";
        return i < args.Length;
    }
}