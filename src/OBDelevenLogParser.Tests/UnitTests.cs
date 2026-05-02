namespace OBDelevenLogParser.Tests;

public class UnitTests
{
    // ── vehicle header ────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ReturnsCorrectVehicleInfo()
    {
        var log = OBDParser.Parse(MinimalLog());

        Assert.Equal("WVWZZZ7RZJY048321", log.Vehicle.Vin);
        Assert.Equal("Volkswagen Golf R",         log.Vehicle.Car);
        Assert.Equal(2018,                 log.Vehicle.Year);
        Assert.Equal(87432,               log.Vehicle.Mileage);
    }

    [Fact]
    public void Parse_ReturnsCorrectLogDate()
    {
        var log = OBDParser.Parse(MinimalLog());

        Assert.Equal(new DateTime(2026, 4, 18, 14, 17, 58), log.LogDate);
    }

    // ── modules ───────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ReturnsCorrectModuleCount()
    {
        var log = OBDParser.Parse(MinimalLog());

        Assert.Single(log.Modules);
    }

    [Fact]
    public void Parse_ReturnsCorrectModuleMetadata()
    {
        var module = OBDParser.Parse(MinimalLog()).Modules[0];

        Assert.Equal("01 Engine",      module.Id);
        Assert.Equal("R4 2.0l TDI",    module.SystemDescription);
        Assert.Equal("05L906022JN",    module.SoftwareNumber);
    }

    [Fact]
    public void Parse_ModuleWithNoFaults_ReturnEmptyFaultList()
    {
        var log = OBDParser.Parse(LogWithNoFaults());

        Assert.Empty(log.Modules[0].Faults);
    }

    // ── faults ────────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ReturnsCorrectFaultCount()
    {
        var log = OBDParser.Parse(MinimalLog());

        Assert.Equal(3, log.Modules[0].Faults.Count);
    }

    [Theory]
    [InlineData(0, "P008700", "Fuel Rail/System Pressure - Too Low",        "Intermittent")]
    [InlineData(1, "P008700", "Fuel Rail/System Pressure - Too Low",        "Intermittent")]
    [InlineData(2, "P008A00", "Low Pressure Fuel System Pressure - Too Low", "Intermittent")]
    public void Parse_ReturnsCorrectFaultCodeAndDescription(int index, string code, string desc, string status)
    {
        var faults = OBDParser.Parse(MinimalLog()).Modules[0].Faults;

        Assert.Equal(code,   faults[index].Code);
        Assert.Equal(desc,   faults[index].Description);
        Assert.Equal(status, faults[index].Status);
    }

    // ── snapshot ──────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ReturnsCorrectSnapshotFields()
    {
        var snapshot = OBDParser.Parse(MinimalLog()).Modules[0].Faults[0].Snapshot;

        Assert.Equal(2,      snapshot.Priority);
        Assert.Equal(1,      snapshot.FrequencyCounter);
        Assert.Equal(30,     snapshot.UnlearningCounter);
        Assert.Equal(131206, snapshot.MileageKm);
        Assert.Equal(436.00,  snapshot.EngineRpm);
        Assert.Equal(0,      snapshot.VehicleSpeedKmh);
        Assert.Equal(20,     snapshot.CoolantTempC);
        Assert.Equal(21,     snapshot.IntakeAirTempC);
        Assert.Equal(990,    snapshot.AmbientPressureMbar);
        Assert.Equal(11.68,  snapshot.Voltage, precision: 2);
    }

    [Fact]
    public void Parse_ReturnsCorrectSnapshotDate()
    {
        var snapshot = OBDParser.Parse(MinimalLog()).Modules[0].Faults[0].Snapshot;

        Assert.Equal(new DateTime(2026, 3, 23, 14, 2, 52), snapshot.Date);
    }

    // ── edge cases ────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_DtcWithHexDigitsInCode_IsRecognised()
    {
        // P008A00 contains 'A' — must not be dropped
        var faults = OBDParser.Parse(MinimalLog()).Modules[0].Faults;

        Assert.Contains(faults, f => f.Code == "P008A00");
    }

    [Fact]
    public void Parse_DuplicateFaultCode_CreatesMultipleEntries()
    {
        // P008700 appears twice in the log with different snapshots
        var faults = OBDParser.Parse(MinimalLog()).Modules[0].Faults;
        var p008700 = faults.Where(f => f.Code == "P008700").ToList();

        Assert.Equal(2, p008700.Count);
        Assert.NotEqual(p008700[0].Snapshot.EngineRpm, p008700[1].Snapshot.EngineRpm);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyModules()
    {
        var log = OBDParser.Parse([]);

        Assert.Empty(log.Modules);
    }

    [Fact]
    public void Parse_MultipleModules_ReturnsAllModules()
    {
        var log = OBDParser.Parse(MultiModuleLog());

        Assert.Equal(2, log.Modules.Count);
    }

    // ── test data ─────────────────────────────────────────────────────────────

    static string[] MinimalLog() =>
    [
        "OBDeleven vehicle history log",
        "Date: 2026-04-18 14:17:58",
        "",
        "	VIN: WVWZZZ7RZJY048321",
        "	Car: Volkswagen Golf R",
        "	Year: 2018",
        "	Body type: UNKNOWN",
        "	Engine: DYS 221 kW (300 hp) 2.0l",
        "	Mileage: 87432 km",
        "",
        "---------------------------------------------------------------",
        "01 Engine",
        "		System description: R4 2.0l TDI",
        "		Software number: 05L906022JN",
        "",
        "		Faults:",
        "				P008700 - Fuel Rail/System Pressure - Too Low",
        "				Intermittent",
        "					Priority - 2 ",
        "					Malfunction frequency counter - 1 ",
        "					Unlearning counter - 30 ",
        "					km-Mileage - 131206 km",
        "					Engine speed - 436.00 1/min",
        "					Normed load value - 99.6 %",
        "					Vehicle speed - 0 km/h",
        "					Coolant temperature - 20 °C",
        "					Intake air temperature - 21 °C",
        "					Ambient air pressure - 990 mbar",
        "					Voltage terminal 30 - 11.680 V",
        "					Dynamic environmental data - 20961E44D20BAA",
        "					date - 2026-03-23 14:02:52 ",
        "",
        "				P008700 - Fuel Rail/System Pressure - Too Low",
        "				Intermittent",
        "					Priority - 2 ",
        "					Malfunction frequency counter - 1 ",
        "					Unlearning counter - 30 ",
        "					km-Mileage - 131206 km",
        "					Engine speed - 641.00 1/min",
        "					Normed load value - 99.6 %",
        "					Vehicle speed - 0 km/h",
        "					Coolant temperature - 20 °C",
        "					Intake air temperature - 21 °C",
        "					Ambient air pressure - 990 mbar",
        "					Voltage terminal 30 - 11.700 V",
        "					Dynamic environmental data - 20961E44D20BAA",
        "					date - 2026-03-23 14:02:52 ",
        "",
        "				P008A00 - Low Pressure Fuel System Pressure - Too Low",
        "				Intermittent",
        "					Priority - 2 ",
        "					Malfunction frequency counter - 1 ",
        "					Unlearning counter - 30 ",
        "					km-Mileage - 131206 km",
        "					Engine speed - 579.00 1/min",
        "					Normed load value - 99.6 %",
        "					Vehicle speed - 0 km/h",
        "					Coolant temperature - 20 °C",
        "					Intake air temperature - 21 °C",
        "					Ambient air pressure - 990 mbar",
        "					Voltage terminal 30 - 11.780 V",
        "					Dynamic environmental data - 20961E44D20BAA",
        "					date - 2026-03-23 14:02:52 ",
    ];

    static string[] LogWithNoFaults() =>
    [
        "OBDeleven vehicle history log",
        "Date: 2026-04-18 14:17:58",
        "	VIN: WVWZZZ7RZJY048321",
        "	Car: Volkswagen",
        "	Year: 2018",
        "	Engine: DYS 221 kW (300 hp) 2.0l",
        "	Mileage: 87432 km",
        "01 Engine",
        "		System description: R4 2.0l TDI",
        "		Software number: 05L906022JN",
        "		Faults:",
    ];

    static string[] MultiModuleLog() =>
    [
        "OBDeleven vehicle history log",
        "Date: 2026-04-18 14:17:58",
        "	VIN: WVWZZZ7RZJY048321",
        "	Car: Volkswagen",
        "	Year: 2018",
        "	Engine: DYS 221 kW (300 hp) 2.0l",
        "	Mileage: 87432 km",
        "01 Engine",
        "		System description: R4 2.0l TDI",
        "		Software number: 05L906022JN",
        "		Faults:",
        "19 CAN Gateway",
        "		System description: CAN Gateway",
        "		Software number: 5Q0907530AE",
        "		Faults:",
    ];
}