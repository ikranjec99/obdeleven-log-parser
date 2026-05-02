# OBDeleven Log Parser

A minimal .NET 10 console app that parses OBDeleven vehicle diagnostic log files and outputs structured JSON.

## Usage

```bash
dotnet run -- path/to/OBDeleven_Log.txt
```

## Output

Parsed log is serialized to JSON with typed fields — no raw strings or magic keys. Each fault entry includes a fully typed snapshot (RPM, temperatures, voltages, etc.) extracted from the freeze frame data.

```json
{
  "LogDate": "2026-04-18T14:17:58",
  "Vehicle": 
  {
    "Vin": "...",
    "Car": "Volkswagen",
    "Year": 2022,
    ...
  },
  "Modules": [
    {
      "Id": "01 Engine",
      "SystemDescription": "R4 2.0l TDI",
      "Faults": [
        {
          "Code": "P008700",
          "Description": "Fuel Rail/System Pressure - Too Low",
          "Status": "Intermittent",
          "Snapshot": { "Priority": 2, "EngineRpm": 436, "CoolantTempC": 20, ... }
        }
      ]
    }
  ]
}
```

## Requirements

- .NET 10 SDK
- OBDeleven log file exported from the [OBDeleven app](https://obdeleven.com)
