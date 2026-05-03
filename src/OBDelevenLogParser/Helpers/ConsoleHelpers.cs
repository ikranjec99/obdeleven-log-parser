using OBDelevenLogParser.Models;
using Spectre.Console;

namespace OBDelevenLogParser.Helpers;

public static class ConsoleHelpers
{
    static readonly IAnsiConsole PrettyConsole = AnsiConsole.Create(new AnsiConsoleSettings
    {
        Out = new AnsiConsoleOutput(Console.Error)
    });

    public static void WriteHeader()
    {
        PrettyConsole.Write(
            new FigletText("OBDelevenLogParser")
                .LeftJustified()
                .Color(Color.Aqua));

        PrettyConsole.MarkupLine("[dim]Diagnostic logs to structured JSON[/]");
        PrettyConsole.WriteLine();
    }

    public static void WriteError(string message)
    {
        var usage = new Grid()
            .AddColumn()
            .AddRow($"[red]{Escape(message)}[/]")
            .AddEmptyRow()
            .AddRow("[bold]Usage[/]")
            .AddRow($"[grey]{Escape("dotnet run -- --input path/to/OBDeleven_Log.txt")}[/]")
            .AddRow($"[grey]{Escape("dotnet run -- --input path/to/OBDeleven_Log.txt -o OBDeleven_Log.json")}[/]")
            .AddRow($"[grey]{Escape("dotnet run -- path/to/OBDeleven_Log.txt -o OBDeleven_Log.json")}[/]");

        PrettyConsole.Write(new Panel(usage)
            .Header("[bold red]Error[/]")
            .RoundedBorder()
            .BorderColor(Color.Red));
    }

    public static void WriteSummary(OBDLog log, string inputPath, string? outputPath)
    {
        var modulesWithFaults = log.Modules.Count(module => module.Faults.Count > 0);
        var faultCount = log.Modules.Sum(module => module.Faults.Count);

        var vehicle = new Grid()
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn()
            .AddRow("[bold]Vehicle[/]", Escape(log.Vehicle.Car))
            .AddRow("[bold]VIN[/]", Escape(log.Vehicle.Vin))
            .AddRow("[bold]Year[/]", log.Vehicle.Year.ToString())
            .AddRow("[bold]Mileage[/]", $"{log.Vehicle.Mileage:N0} km")
            .AddRow("[bold]Engine[/]", Escape(log.Vehicle.Engine));

        var summary = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddColumn("[grey]Metric[/]")
            .AddColumn("[grey]Value[/]");

        summary.AddRow("Input", Escape(inputPath));
        summary.AddRow("Output", outputPath is null ? "[dim]stdout[/]" : Escape(outputPath));
        summary.AddRow("Log date", log.LogDate.ToString("yyyy-MM-dd HH:mm:ss"));
        summary.AddRow("Modules parsed", log.Modules.Count.ToString());
        summary.AddRow("Modules with faults", modulesWithFaults.ToString());
        summary.AddRow("Total faults", faultCount == 0 ? "[green]0[/]" : $"[yellow]{faultCount}[/]");

        PrettyConsole.Write(new Panel(vehicle)
            .Header("[bold aqua]Vehicle[/]")
            .RoundedBorder()
            .BorderColor(Color.Aqua));

        PrettyConsole.Write(summary);
        WriteFaultOverview(log);
        PrettyConsole.WriteLine();
    }

    static void WriteFaultOverview(OBDLog log)
    {
        var faults = log.Modules
            .SelectMany(module => module.Faults.Select(fault => new
            {
                Module = module.Id,
                fault.Code,
                fault.Status,
                fault.Description
            }))
            .ToList();

        if (faults.Count == 0)
        {
            PrettyConsole.Write(new Panel("[green]No faults found.[/]")
                .Header("[bold green]Fault Overview[/]")
                .RoundedBorder()
                .BorderColor(Color.Green));

            return;
        }

        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Yellow)
            .Expand()
            .Title("[bold yellow]Fault Overview[/]")
            .AddColumn(new TableColumn("[grey]Module[/]").NoWrap())
            .AddColumn(new TableColumn("[grey]Code[/]").NoWrap())
            .AddColumn(new TableColumn("[grey]Status[/]").NoWrap())
            .AddColumn("[grey]Description[/]");

        foreach (var fault in faults)
        {
            table.AddRow(
                Escape(fault.Module),
                $"[bold]{Escape(fault.Code)}[/]",
                FormatStatus(fault.Status),
                Escape(fault.Description));
        }

        PrettyConsole.Write(table);
    }

    static string FormatStatus(string status)
    {
        var escaped = Escape(status);
        var normalized = status.ToLowerInvariant();

        if (normalized.Contains("intermittent", StringComparison.Ordinal))
            return $"[yellow]{escaped}[/]";

        if (normalized.Contains("static", StringComparison.Ordinal) ||
            normalized.Contains("active", StringComparison.Ordinal))
            return $"[red]{escaped}[/]";

        if (normalized.Contains("no fault", StringComparison.Ordinal) ||
            normalized.Contains("passive", StringComparison.Ordinal))
            return $"[green]{escaped}[/]";

        return escaped;
    }

    static string Escape(string value) => Markup.Escape(value);
}
