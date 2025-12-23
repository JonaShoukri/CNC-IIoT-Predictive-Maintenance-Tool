using System.Diagnostics;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_;

public static class Program
{
    private static readonly ProductionFloor Floor = ProductionFloor.Instance;
    private static bool _running = true;
    private static readonly List<string> AlertLog = new();
    private static double _warningThreshold = 0.8;
    private static double _criticalThreshold = 0.9;

    public static void Main()
    {
        Console.Title = "CNC Production Floor - Predictive Maintenance Digital Twin";
        Console.CursorVisible = false;

        Floor.OnBearingFailed += (cnc, bearingIndex) =>
        {
            var alert = $"[{DateTime.Now:HH:mm:ss}] FAILURE: Machine {cnc.Model} (#{cnc.Id.ToString()[..8]}) - Bearing {bearingIndex} ({cnc.Bearings[bearingIndex].Position}) has FAILED!";
            AlertLog.Add(alert);
            if (AlertLog.Count > 50) AlertLog.RemoveAt(0);
        };

        ShowWelcomeScreen();

        while (_running)
        {
            ShowMainMenu();
        }

        Console.CursorVisible = true;
        Console.Clear();
        Console.WriteLine("Production floor simulation terminated. Goodbye!");
    }

    private static void ShowWelcomeScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
   ╔═══════════════════════════════════════════════════════════════════════════════╗
   ║                                                                               ║
   ║     ██████╗███╗   ██╗ ██████╗    ██████╗ ██████╗  ██████╗ ██████╗            ║
   ║    ██╔════╝████╗  ██║██╔════╝    ██╔══██╗██╔══██╗██╔═══██╗██╔══██╗           ║
   ║    ██║     ██╔██╗ ██║██║         ██████╔╝██████╔╝██║   ██║██║  ██║           ║
   ║    ██║     ██║╚██╗██║██║         ██╔═══╝ ██╔══██╗██║   ██║██║  ██║           ║
   ║    ╚██████╗██║ ╚████║╚██████╗    ██║     ██║  ██║╚██████╔╝██████╔╝           ║
   ║     ╚═════╝╚═╝  ╚═══╝ ╚═════╝    ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═════╝            ║
   ║                                                                               ║
   ║           PREDICTIVE MAINTENANCE DIGITAL TWIN SYSTEM                          ║
   ║                         IIoT Simulation Platform                              ║
   ║                                                                               ║
   ╚═══════════════════════════════════════════════════════════════════════════════╝
");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   Loading ML Models...");
        Console.ResetColor();
        Console.WriteLine("   • 26 Degradation prediction models");
        Console.WriteLine("   • 1 RUL (Remaining Useful Life) prediction model");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   System Ready.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("   Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void ShowMainMenu()
    {
        Console.Clear();
        DrawHeader("MAIN CONTROL PANEL");

        var activeCount = Floor.ActiveMachineCount;
        var failedCount = Floor.FailedMachineCount;
        var totalMachines = Floor.Machines.Count;
        var offCount = totalMachines - activeCount - failedCount;

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"  Machines: {totalMachines} total | ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{activeCount} active");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" | ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{offCount} off");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{failedCount} failed");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"  Iteration: {Floor.TotalIterations} | Time Multiplier: {Floor.TimeMultiplier}x");
        Console.WriteLine();

        var warningBearings = Floor.GetBearingsNeedingAttention(_warningThreshold).ToList();
        if (warningBearings.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ {warningBearings.Count} bearing(s) need attention!");
            Console.ResetColor();
        }

        Console.WriteLine();
        DrawMenuSection("DASHBOARD");
        Console.WriteLine("  [1] Production Floor Overview");
        Console.WriteLine("  [2] Machine Status Dashboard");
        Console.WriteLine("  [3] Bearing Health Monitor");
        Console.WriteLine("  [4] Alert Log");

        DrawMenuSection("MACHINE MANAGEMENT");
        Console.WriteLine("  [5] Add New CNC Machine");
        Console.WriteLine("  [6] Remove CNC Machine");
        Console.WriteLine("  [7] Toggle Machine Power");
        Console.WriteLine("  [8] Inspect Machine Details");

        DrawMenuSection("SIMULATION");
        Console.WriteLine("  [9] Run Real-Time Simulation");
        Console.WriteLine("  [A] Run Accelerated Simulation");
        Console.WriteLine("  [B] Step Simulation (Single Tick)");
        Console.WriteLine("  [C] Configure Time Multiplier");

        DrawMenuSection("MAINTENANCE");
        Console.WriteLine("  [D] Replace Bearing");
        Console.WriteLine("  [E] Maintenance Scheduler");

        DrawMenuSection("SYSTEM");
        Console.WriteLine("  [S] Settings");
        Console.WriteLine("  [R] Reset Production Floor");
        Console.WriteLine("  [Q] Quit");

        Console.WriteLine();
        Console.Write("  Select option: ");

        var key = Console.ReadKey(true);
        HandleMainMenuInput(key.KeyChar);
    }

    private static void HandleMainMenuInput(char input)
    {
        switch (char.ToUpper(input))
        {
            case '1': ShowProductionFloorOverview(); break;
            case '2': ShowMachineStatusDashboard(); break;
            case '3': ShowBearingHealthMonitor(); break;
            case '4': ShowAlertLog(); break;
            case '5': AddNewMachine(); break;
            case '6': RemoveMachine(); break;
            case '7': ToggleMachinePower(); break;
            case '8': InspectMachine(); break;
            case '9': RunRealTimeSimulation(); break;
            case 'A': RunAcceleratedSimulation(); break;
            case 'B': StepSimulation(); break;
            case 'C': ConfigureTimeMultiplier(); break;
            case 'D': ReplaceBearing(); break;
            case 'E': MaintenanceScheduler(); break;
            case 'S': ShowSettings(); break;
            case 'R': ResetProductionFloor(); break;
            case 'Q': _running = false; break;
        }
    }

    private static void ShowProductionFloorOverview()
    {
        Console.Clear();
        DrawHeader("PRODUCTION FLOOR OVERVIEW");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.WriteLine("  Press [5] from main menu to add machines.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine("  │                        PRODUCTION FLOOR LAYOUT                         │");
        Console.WriteLine("  └─────────────────────────────────────────────────────────────────────────┘");
        Console.WriteLine();

        int col = 0;
        foreach (var (cnc, index) in Floor.Machines.Select((c, i) => (c, i)))
        {
            if (col == 0) Console.Write("  ");

            DrawMachineBox(cnc, index);
            Console.Write("  ");

            col++;
            if (col >= 3)
            {
                Console.WriteLine();
                Console.WriteLine();
                col = 0;
            }
        }

        Console.WriteLine();
        Console.WriteLine();
        DrawLegend();
        WaitForKey();
    }

    private static void DrawMachineBox(CNC cnc, int index)
    {
        var bgColor = cnc.HasFailed ? ConsoleColor.DarkRed :
                      cnc.IsOn ? ConsoleColor.DarkGreen :
                      ConsoleColor.DarkGray;

        var statusIcon = cnc.HasFailed ? "✖" : cnc.IsOn ? "●" : "○";
        var shortId = cnc.Id.ToString()[..6];

        Console.BackgroundColor = bgColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" [{index}] {cnc.Model,-18} {statusIcon} ");
        Console.ResetColor();
    }

    private static void DrawLegend()
    {
        Console.WriteLine("  Legend:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  ● Active  ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("○ Off  ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("✖ Failed");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void ShowMachineStatusDashboard()
    {
        Console.Clear();
        DrawHeader("MACHINE STATUS DASHBOARD");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("  ┌─────┬──────────────────────┬────────────┬──────────┬──────────────┬─────────────────┐");
        Console.WriteLine("  │ IDX │ MODEL                │ STATUS     │ BEARINGS │ LOWEST HEALTH│ REVOLUTIONS     │");
        Console.WriteLine("  ├─────┼──────────────────────┼────────────┼──────────┼──────────────┼─────────────────┤");

        foreach (var (cnc, index) in Floor.Machines.Select((c, i) => (c, i)))
        {
            var status = cnc.HasFailed ? "FAILED" : cnc.IsOn ? "RUNNING" : "OFF";
            var statusColor = cnc.HasFailed ? ConsoleColor.Red : cnc.IsOn ? ConsoleColor.Green : ConsoleColor.Gray;

            var lowestHealth = cnc.Bearings.Min(b => b.HealthPercentage);
            var healthColor = lowestHealth < 10 ? ConsoleColor.Red :
                             lowestHealth < 30 ? ConsoleColor.Yellow :
                             ConsoleColor.Green;

            var totalRevs = cnc.Bearings.Max(b => b.State.Revolutions);

            Console.Write($"  │ {index,3} │ {cnc.Model,-20} │ ");
            Console.ForegroundColor = statusColor;
            Console.Write($"{status,-10}");
            Console.ResetColor();
            Console.Write($" │ {cnc.Bearings.Length,8} │ ");
            Console.ForegroundColor = healthColor;
            Console.Write($"{lowestHealth,10:F1}%");
            Console.ResetColor();
            Console.WriteLine($"  │ {totalRevs,15:N0} │");
        }

        Console.WriteLine("  └─────┴──────────────────────┴────────────┴──────────┴──────────────┴─────────────────┘");
        Console.WriteLine();

        Console.WriteLine($"  Total Machines: {Floor.Machines.Count}");
        Console.WriteLine($"  Active: {Floor.ActiveMachineCount} | Failed: {Floor.FailedMachineCount} | Off: {Floor.Machines.Count - Floor.ActiveMachineCount - Floor.FailedMachineCount}");
        Console.WriteLine($"  Total Bearings: {Floor.TotalBearingCount}");
        Console.WriteLine($"  Total Iterations: {Floor.TotalIterations}");

        WaitForKey();
    }

    private static void ShowBearingHealthMonitor()
    {
        Console.Clear();
        DrawHeader("BEARING HEALTH MONITOR");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        var allBearings = Floor.Machines
            .SelectMany(c => c.Bearings.Select((b, i) => (Machine: c, Bearing: b, Index: i)))
            .OrderByDescending(x => x.Bearing.RUL)
            .ToList();

        Console.WriteLine();
        Console.WriteLine("  All Bearings Sorted by Degradation (Most Degraded First):");
        Console.WriteLine();
        Console.WriteLine("  ┌──────────────────────┬────────────┬─────────────┬────────────┬──────────────────┐");
        Console.WriteLine("  │ MACHINE              │ POSITION   │ HEALTH %    │ STATUS     │ RUL VALUE        │");
        Console.WriteLine("  ├──────────────────────┼────────────┼─────────────┼────────────┼──────────────────┤");

        foreach (var item in allBearings.Take(20))
        {
            var health = item.Bearing.HealthPercentage;
            var status = item.Bearing.GetHealthStatus();
            var statusColor = status switch
            {
                "FAILED" => ConsoleColor.DarkRed,
                "CRITICAL" => ConsoleColor.Red,
                "WARNING" => ConsoleColor.Yellow,
                "FAIR" => ConsoleColor.DarkYellow,
                _ => ConsoleColor.Green
            };

            Console.Write($"  │ {item.Machine.Model,-20} │ {item.Bearing.Position,-10} │ ");
            Console.ForegroundColor = statusColor;
            Console.Write($"{health,9:F1}%");
            Console.ResetColor();
            Console.Write($"  │ ");
            Console.ForegroundColor = statusColor;
            Console.Write($"{status,-10}");
            Console.ResetColor();
            Console.WriteLine($" │ {item.Bearing.RUL,16:F4} │");
        }

        Console.WriteLine("  └──────────────────────┴────────────┴─────────────┴────────────┴──────────────────┘");

        if (allBearings.Count > 20)
        {
            Console.WriteLine($"  ... and {allBearings.Count - 20} more bearings");
        }

        Console.WriteLine();

        var criticalCount = allBearings.Count(x => x.Bearing.RUL >= _criticalThreshold);
        var warningCount = allBearings.Count(x => x.Bearing.RUL >= _warningThreshold && x.Bearing.RUL < _criticalThreshold);

        if (criticalCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ⚠ CRITICAL: {criticalCount} bearing(s) at risk of imminent failure!");
            Console.ResetColor();
        }
        if (warningCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ WARNING: {warningCount} bearing(s) showing significant wear.");
            Console.ResetColor();
        }

        WaitForKey();
    }

    private static void ShowAlertLog()
    {
        Console.Clear();
        DrawHeader("ALERT LOG");
        Console.WriteLine();

        if (!AlertLog.Any())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  No alerts recorded. System operating normally.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"  Last {AlertLog.Count} alerts:");
            Console.WriteLine();

            foreach (var alert in AlertLog.AsEnumerable().Reverse())
            {
                if (alert.Contains("FAILURE"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (alert.Contains("WARNING"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.WriteLine($"  {alert}");
                Console.ResetColor();
            }
        }

        WaitForKey();
    }

    private static void AddNewMachine()
    {
        Console.Clear();
        DrawHeader("ADD NEW CNC MACHINE");
        Console.WriteLine();
        Console.WriteLine("  Available Models:");
        Console.WriteLine("  [1] Haas VF-2");
        Console.WriteLine("  [2] DMG MORI DMU 50");
        Console.WriteLine("  [3] Mazak Integrex i-200");
        Console.WriteLine("  [4] Okuma GENOS M560-V");
        Console.WriteLine("  [5] Fanuc RoboDrill");
        Console.WriteLine("  [6] Random Model");
        Console.WriteLine("  [C] Custom Name");
        Console.WriteLine("  [0] Cancel");
        Console.WriteLine();
        Console.Write("  Select model: ");

        var key = Console.ReadKey(true);
        string? modelName = key.KeyChar switch
        {
            '1' => "Haas VF-2",
            '2' => "DMG MORI DMU 50",
            '3' => "Mazak Integrex i-200",
            '4' => "Okuma GENOS M560-V",
            '5' => "Fanuc RoboDrill",
            '6' => null,
            'c' or 'C' => GetCustomModelName(),
            '0' => "CANCEL",
            _ => null
        };

        if (modelName == "CANCEL") return;

        var cnc = new CNC(modelName);
        Floor.AddCnc(cnc);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ Added {cnc.Model} with {cnc.Bearings.Length} bearings");
        Console.WriteLine($"    ID: {cnc.Id}");
        Console.ResetColor();

        Thread.Sleep(1500);
    }

    private static string GetCustomModelName()
    {
        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter custom model name: ");
        var name = Console.ReadLine()?.Trim();
        Console.CursorVisible = false;
        return string.IsNullOrEmpty(name) ? "Custom CNC" : name;
    }

    private static void RemoveMachine()
    {
        Console.Clear();
        DrawHeader("REMOVE CNC MACHINE");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines to remove.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        ListMachinesCompact();

        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter machine index to remove (or 'C' to cancel): ");
        var input = Console.ReadLine()?.Trim().ToUpper();
        Console.CursorVisible = false;

        if (input == "C" || string.IsNullOrEmpty(input)) return;

        if (int.TryParse(input, out int index))
        {
            var cnc = Floor.GetCncByIndex(index);
            if (cnc != null)
            {
                Floor.RemoveCnc(cnc.Id);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ Removed {cnc.Model}");
                Console.ResetColor();
                Thread.Sleep(1000);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid index.");
                Console.ResetColor();
                Thread.Sleep(1000);
            }
        }
    }

    private static void ToggleMachinePower()
    {
        Console.Clear();
        DrawHeader("TOGGLE MACHINE POWER");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        ListMachinesCompact();

        Console.WriteLine();
        Console.WriteLine("  [A] Turn ALL machines ON");
        Console.WriteLine("  [Z] Turn ALL machines OFF");
        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter machine index (or A/Z for all, C to cancel): ");
        var input = Console.ReadLine()?.Trim().ToUpper();
        Console.CursorVisible = false;

        if (input == "C" || string.IsNullOrEmpty(input)) return;

        if (input == "A")
        {
            int turned = 0;
            foreach (var cnc in Floor.Machines.Where(c => !c.IsOn && !c.HasFailed))
            {
                cnc.TurnOn();
                turned++;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ Turned ON {turned} machine(s)");
            Console.ResetColor();
            Thread.Sleep(1000);
            return;
        }

        if (input == "Z")
        {
            int turned = 0;
            foreach (var cnc in Floor.Machines.Where(c => c.IsOn))
            {
                cnc.TurnOff();
                turned++;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ Turned OFF {turned} machine(s)");
            Console.ResetColor();
            Thread.Sleep(1000);
            return;
        }

        if (int.TryParse(input, out int index))
        {
            var cnc = Floor.GetCncByIndex(index);
            if (cnc != null)
            {
                if (cnc.HasFailed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Cannot turn on a failed machine. Replace the failed bearing first.");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    return;
                }

                cnc.TogglePower();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ {cnc.Model} is now {(cnc.IsOn ? "ON" : "OFF")}");
                Console.ResetColor();
                Thread.Sleep(1000);
            }
        }
    }

    private static void InspectMachine()
    {
        Console.Clear();
        DrawHeader("INSPECT MACHINE");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        ListMachinesCompact();

        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter machine index to inspect (or 'C' to cancel): ");
        var input = Console.ReadLine()?.Trim().ToUpper();
        Console.CursorVisible = false;

        if (input == "C" || string.IsNullOrEmpty(input)) return;

        if (int.TryParse(input, out int index))
        {
            var cnc = Floor.GetCncByIndex(index);
            if (cnc != null)
            {
                ShowMachineDetails(cnc);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid index.");
                Console.ResetColor();
                Thread.Sleep(1000);
            }
        }
    }

    private static void ShowMachineDetails(CNC cnc)
    {
        Console.Clear();
        DrawHeader($"MACHINE DETAILS: {cnc.Model}");
        Console.WriteLine();

        Console.WriteLine($"  ID:       {cnc.Id}");
        Console.WriteLine($"  Model:    {cnc.Model}");
        Console.Write($"  Status:   ");
        if (cnc.HasFailed)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FAILED");
        }
        else if (cnc.IsOn)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("RUNNING");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("OFF");
        }
        Console.ResetColor();

        Console.WriteLine($"  Bearings: {cnc.Bearings.Length}");
        Console.WriteLine();

        Console.WriteLine("  ┌─────────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine("  │                           BEARING DETAILS                              │");
        Console.WriteLine("  └─────────────────────────────────────────────────────────────────────────┘");
        Console.WriteLine();

        for (int i = 0; i < cnc.Bearings.Length; i++)
        {
            var b = cnc.Bearings[i];
            var status = b.GetHealthStatus();
            var statusColor = status switch
            {
                "FAILED" => ConsoleColor.DarkRed,
                "CRITICAL" => ConsoleColor.Red,
                "WARNING" => ConsoleColor.Yellow,
                "FAIR" => ConsoleColor.DarkYellow,
                _ => ConsoleColor.Green
            };

            var isFailed = cnc.FailedBearingIndex == i;

            Console.Write($"  Bearing {i} ({b.Position})");
            if (isFailed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [FAILED COMPONENT]");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.Write($"    Health: ");
            Console.ForegroundColor = statusColor;
            DrawProgressBar(b.HealthPercentage, 30);
            Console.Write($" {b.HealthPercentage:F1}% ({status})");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine($"    RUL Value:   {b.RUL:F6}");
            Console.WriteLine($"    Revolutions: {b.State.Revolutions:N0}");
            Console.WriteLine();

            Console.WriteLine("    Key Metrics:");
            Console.WriteLine($"      RMS: {b.State.Rms:F4}    Peak: {b.State.Peak:F4}    Kurtosis: {b.State.Kurtosis:F4}");
            Console.WriteLine($"      BPFO Energy: {b.State.EnergyBPFO:F6}    BPFI Energy: {b.State.EnergyBPFI:F6}");
            Console.WriteLine();
        }

        Console.WriteLine("  Press [B] to view full bearing data, or any other key to return...");
        var key = Console.ReadKey(true);
        if (key.KeyChar == 'b' || key.KeyChar == 'B')
        {
            ShowFullBearingData(cnc);
        }
    }

    private static void ShowFullBearingData(CNC cnc)
    {
        for (int i = 0; i < cnc.Bearings.Length; i++)
        {
            Console.Clear();
            DrawHeader($"FULL BEARING DATA: {cnc.Model} - Bearing {i} ({cnc.Bearings[i].Position})");
            Console.WriteLine();

            var s = cnc.Bearings[i].State;

            Console.WriteLine("  Statistical Features:");
            Console.WriteLine($"    RMS:              {s.Rms,12:F6}");
            Console.WriteLine($"    Peak:             {s.Peak,12:F6}");
            Console.WriteLine($"    Crest Factor:     {s.CrestFactor,12:F6}");
            Console.WriteLine($"    Kurtosis:         {s.Kurtosis,12:F6}");
            Console.WriteLine($"    Skewness:         {s.Skewness,12:F6}");
            Console.WriteLine($"    Std Dev:          {s.StdDev,12:F6}");
            Console.WriteLine($"    Peak-to-Peak:     {s.PeakToPeak,12:F6}");
            Console.WriteLine($"    Mean:             {s.Mean,12:F6}");
            Console.WriteLine($"    Variance:         {s.Variance,12:F6}");
            Console.WriteLine();

            Console.WriteLine("  Frequency Domain:");
            Console.WriteLine($"    Dominant Freq:    {s.DominantFrequency_Hz,12:F2} Hz");
            Console.WriteLine($"    Dominant Mag:     {s.DominantFrequency_Mag,12:F6}");
            Console.WriteLine();

            Console.WriteLine("  Energy Bands:");
            Console.WriteLine($"    0-500 Hz:         {s.Energy_0_500Hz,12:F6}");
            Console.WriteLine($"    500-1000 Hz:      {s.Energy_500_1000Hz,12:F6}");
            Console.WriteLine($"    1000-2000 Hz:     {s.Energy_1000_2000Hz,12:F6}");
            Console.WriteLine($"    2000-4000 Hz:     {s.Energy_2000_4000Hz,12:F6}");
            Console.WriteLine($"    4000-6000 Hz:     {s.Energy_4000_6000Hz,12:F6}");
            Console.WriteLine($"    6000-8000 Hz:     {s.Energy_6000_8000Hz,12:F6}");
            Console.WriteLine($"    8000-10240 Hz:    {s.Energy_8000_10240Hz,12:F6}");
            Console.WriteLine();

            Console.WriteLine("  Bearing Fault Frequencies:");
            Console.WriteLine($"    BPFO:             {s.EnergyBPFO,12:F6}     BPFO 2x: {s.EnergyBPFO_2x,12:F6}     BPFO 3x: {s.EnergyBPFO_3x,12:F6}");
            Console.WriteLine($"    BPFI:             {s.EnergyBPFI,12:F6}     BPFI 2x: {s.EnergyBPFI_2x,12:F6}     BPFI 3x: {s.EnergyBPFI_3x,12:F6}");
            Console.WriteLine($"    BSF:              {s.EnergyBSF,12:F6}");
            Console.WriteLine($"    FTF:              {s.EnergyFTF,12:F6}");
            Console.WriteLine();

            Console.WriteLine("  Operational:");
            Console.WriteLine($"    Revolutions:      {s.Revolutions,12:N0}");
            Console.WriteLine($"    RUL:              {s.RUL,12:F6}");

            if (i < cnc.Bearings.Length - 1)
            {
                Console.WriteLine();
                Console.WriteLine("  Press any key for next bearing...");
                Console.ReadKey(true);
            }
            else
            {
                WaitForKey();
            }
        }
    }

    private static void RunRealTimeSimulation()
    {
        Console.Clear();
        DrawHeader("REAL-TIME SIMULATION");

        if (!Floor.Machines.Any(c => c.IsOn))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No active machines. Turn on machines first.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Floor.SetTimeMultiplier(1.0);
        Console.WriteLine();
        Console.WriteLine("  Real-time simulation running (7.5 minutes per iteration).");
        Console.WriteLine("  Press any key to stop...");
        Console.WriteLine();

        RunSimulationLoop();
    }

    private static void RunAcceleratedSimulation()
    {
        Console.Clear();
        DrawHeader("ACCELERATED SIMULATION");

        if (!Floor.Machines.Any(c => c.IsOn))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No active machines. Turn on machines first.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("  Select acceleration factor:");
        Console.WriteLine("  [1] 10x    (45 seconds per iteration)");
        Console.WriteLine("  [2] 60x    (7.5 seconds per iteration)");
        Console.WriteLine("  [3] 450x   (1 second per iteration)");
        Console.WriteLine("  [4] 4500x  (0.1 seconds per iteration)");
        Console.WriteLine("  [5] Custom multiplier");
        Console.WriteLine();
        Console.Write("  Select: ");

        var key = Console.ReadKey(true);
        double multiplier = key.KeyChar switch
        {
            '1' => 10,
            '2' => 60,
            '3' => 450,
            '4' => 4500,
            '5' => GetCustomMultiplier(),
            _ => 60
        };

        Floor.SetTimeMultiplier(multiplier);
        Console.WriteLine();
        Console.WriteLine($"  Running at {multiplier}x speed. Press any key to stop...");
        Console.WriteLine();

        RunSimulationLoop();
    }

    private static double GetCustomMultiplier()
    {
        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter multiplier: ");
        var input = Console.ReadLine();
        Console.CursorVisible = false;

        if (double.TryParse(input, out double mult) && mult > 0)
        {
            return mult;
        }
        return 60;
    }

    private static void RunSimulationLoop()
    {
        var sw = Stopwatch.StartNew();
        double lastTime = 0;
        int lastIteration = Floor.TotalIterations;
        var recentAlerts = new List<string>();

        DrawLiveDashboard(recentAlerts);

        while (!Console.KeyAvailable)
        {
            double currentTime = sw.Elapsed.TotalSeconds;
            double delta = currentTime - lastTime;
            lastTime = currentTime;

            int beforeIteration = Floor.TotalIterations;
            Floor.Tick(delta);

            if (Floor.TotalIterations > lastIteration)
            {
                lastIteration = Floor.TotalIterations;

                foreach (var cnc in Floor.Machines.Where(c => c.HasFailed && c.FailedBearingIndex.HasValue))
                {
                    var alertMsg = $"[Iter {Floor.TotalIterations}] {cnc.Model} - Bearing {cnc.FailedBearingIndex} FAILED";
                    if (!recentAlerts.Contains(alertMsg))
                    {
                        recentAlerts.Insert(0, alertMsg);
                        if (recentAlerts.Count > 5) recentAlerts.RemoveAt(5);
                    }
                }

                DrawLiveDashboard(recentAlerts);
            }

            if (!Floor.Machines.Any(c => c.IsOn && !c.HasFailed))
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  All machines have stopped or failed. Press any key...");
                Console.ResetColor();
                Console.ReadKey(true);
                return;
            }

            Thread.Sleep(16);
        }

        if (Console.KeyAvailable) Console.ReadKey(true);
    }

    private static void DrawLiveDashboard(List<string> recentAlerts)
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ══════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("    LIVE SIMULATION DASHBOARD                              [Press any key to stop]");
        Console.WriteLine("  ══════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();

        var elapsed = TimeSpan.FromSeconds(Floor.TotalIterations * ProductionFloor.SecondsPerIteration);
        Console.WriteLine();
        Console.Write($"  Iteration: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{Floor.TotalIterations}");
        Console.ResetColor();
        Console.Write($"  |  Simulated Time: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{elapsed.Days}d {elapsed.Hours}h {elapsed.Minutes}m");
        Console.ResetColor();
        Console.Write($"  |  Speed: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{Floor.TimeMultiplier}x");
        Console.ResetColor();

        var active = Floor.ActiveMachineCount;
        var failed = Floor.FailedMachineCount;
        var off = Floor.Machines.Count - active - failed;

        Console.Write($"  Machines: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{active} active");
        Console.ResetColor();
        Console.Write(" | ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"{off} off");
        Console.ResetColor();
        Console.Write(" | ");
        Console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Gray;
        Console.WriteLine($"{failed} failed");
        Console.ResetColor();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ─── MACHINE STATUS ───");
        Console.ResetColor();
        Console.WriteLine();

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on floor.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("  ┌─────────────────────────┬──────────┬────────────────────────────────────────────────┐");
            Console.WriteLine("  │ MACHINE                 │ STATUS   │ BEARING HEALTH                                 │");
            Console.WriteLine("  ├─────────────────────────┼──────────┼────────────────────────────────────────────────┤");

            foreach (var cnc in Floor.Machines)
            {
                var status = cnc.HasFailed ? "FAILED" : cnc.IsOn ? "RUNNING" : "OFF";
                var statusColor = cnc.HasFailed ? ConsoleColor.Red : cnc.IsOn ? ConsoleColor.Green : ConsoleColor.DarkGray;

                Console.Write($"  │ {cnc.Model,-23} │ ");
                Console.ForegroundColor = statusColor;
                Console.Write($"{status,-8}");
                Console.ResetColor();
                Console.Write(" │ ");

                for (int i = 0; i < cnc.Bearings.Length; i++)
                {
                    var b = cnc.Bearings[i];
                    var health = b.HealthPercentage;
                    var barColor = health < 10 ? ConsoleColor.DarkRed :
                                   health < 30 ? ConsoleColor.Red :
                                   health < 50 ? ConsoleColor.Yellow :
                                   ConsoleColor.Green;

                    if (cnc.FailedBearingIndex == i)
                        barColor = ConsoleColor.DarkRed;

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"B{i}:");
                    Console.ForegroundColor = barColor;
                    DrawMiniProgressBar(health, 8);
                    Console.ResetColor();
                    Console.Write(" ");
                }

                int padding = 48 - (cnc.Bearings.Length * 12);
                Console.Write(new string(' ', Math.Max(0, padding)));
                Console.WriteLine("│");
            }

            Console.WriteLine("  └─────────────────────────┴──────────┴────────────────────────────────────────────────┘");
        }

        var warnings = Floor.GetBearingsNeedingAttention(_warningThreshold).ToList();
        if (warnings.Any())
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  ─── ALERTS ───");
            Console.ResetColor();
            Console.WriteLine();

            var critical = warnings.Where(x => x.Bearing.RUL >= _criticalThreshold).Take(3).ToList();
            var warning = warnings.Where(x => x.Bearing.RUL < _criticalThreshold).Take(3).ToList();

            foreach (var item in critical)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ⚠ CRITICAL: {item.Machine.Model} - {item.Bearing.Position} at {item.Bearing.HealthPercentage:F0}% health");
                Console.ResetColor();
            }

            foreach (var item in warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  ⚠ WARNING:  {item.Machine.Model} - {item.Bearing.Position} at {item.Bearing.HealthPercentage:F0}% health");
                Console.ResetColor();
            }

            if (warnings.Count > 6)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     ... and {warnings.Count - 6} more");
                Console.ResetColor();
            }
        }

        if (recentAlerts.Any())
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  ─── FAILURE LOG ───");
            Console.ResetColor();
            Console.WriteLine();

            foreach (var alert in recentAlerts)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ✖ {alert}");
                Console.ResetColor();
            }
        }
    }

    private static void DrawMiniProgressBar(double percentage, int width)
    {
        int filled = (int)(percentage / 100 * width);
        Console.Write(new string('█', Math.Min(filled, width)));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', Math.Max(0, width - filled)));
    }

    private static void StepSimulation()
    {
        if (!Floor.Machines.Any(c => c.IsOn && !c.HasFailed))
        {
            Console.Clear();
            DrawHeader("STEP SIMULATION");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No active machines to simulate.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Floor.ForceTick();

        Console.Clear();
        DrawHeader("STEP SIMULATION");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ Completed iteration {Floor.TotalIterations}");
        Console.ResetColor();
        Console.WriteLine();

        foreach (var cnc in Floor.Machines.Where(c => c.IsOn || c.HasFailed))
        {
            var worst = cnc.GetMostDegradedBearing();
            Console.Write($"  {cnc.Model}: ");

            if (cnc.HasFailed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAILED (Bearing {cnc.FailedBearingIndex})");
                Console.ResetColor();
            }
            else
            {
                var health = worst.HealthPercentage;
                Console.ForegroundColor = health < 30 ? ConsoleColor.Red :
                                         health < 50 ? ConsoleColor.Yellow :
                                         ConsoleColor.Green;
                Console.WriteLine($"Lowest bearing health: {health:F1}%");
                Console.ResetColor();
            }
        }

        WaitForKey();
    }

    private static void ConfigureTimeMultiplier()
    {
        Console.Clear();
        DrawHeader("CONFIGURE TIME MULTIPLIER");
        Console.WriteLine();
        Console.WriteLine($"  Current multiplier: {Floor.TimeMultiplier}x");
        Console.WriteLine();
        Console.WriteLine("  Presets:");
        Console.WriteLine("  [1] 1x     (Real-time: 7.5 min/iteration)");
        Console.WriteLine("  [2] 10x    (45 sec/iteration)");
        Console.WriteLine("  [3] 60x    (7.5 sec/iteration)");
        Console.WriteLine("  [4] 450x   (1 sec/iteration)");
        Console.WriteLine("  [5] Custom");
        Console.WriteLine("  [0] Cancel");
        Console.WriteLine();
        Console.Write("  Select: ");

        var key = Console.ReadKey(true);
        double? multiplier = key.KeyChar switch
        {
            '1' => 1,
            '2' => 10,
            '3' => 60,
            '4' => 450,
            '5' => GetCustomMultiplier(),
            '0' => null,
            _ => null
        };

        if (multiplier.HasValue)
        {
            Floor.SetTimeMultiplier(multiplier.Value);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ Time multiplier set to {multiplier.Value}x");
            Console.ResetColor();
            Thread.Sleep(1000);
        }
    }

    private static void ReplaceBearing()
    {
        Console.Clear();
        DrawHeader("REPLACE BEARING");

        if (!Floor.Machines.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No machines on the production floor.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        var machinesWithIssues = Floor.Machines
            .Where(c => c.HasFailed || c.Bearings.Any(b => b.RUL >= _warningThreshold))
            .ToList();

        if (!machinesWithIssues.Any())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  All bearings are in good condition. No replacement needed.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("  Machines needing attention:");
        Console.WriteLine();

        foreach (var (cnc, idx) in machinesWithIssues.Select((c, i) => (c, i)))
        {
            Console.Write($"  [{idx}] {cnc.Model}");
            if (cnc.HasFailed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [FAILED]");
                Console.ResetColor();
            }
            Console.WriteLine();

            for (int i = 0; i < cnc.Bearings.Length; i++)
            {
                var b = cnc.Bearings[i];
                var status = b.GetHealthStatus();
                if (status != "GOOD")
                {
                    Console.ForegroundColor = status switch
                    {
                        "FAILED" => ConsoleColor.DarkRed,
                        "CRITICAL" => ConsoleColor.Red,
                        "WARNING" => ConsoleColor.Yellow,
                        _ => ConsoleColor.Gray
                    };
                    Console.WriteLine($"      Bearing {i} ({b.Position}): {status} - {b.HealthPercentage:F1}% health");
                    Console.ResetColor();
                }
            }
        }

        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter machine index (or 'C' to cancel): ");
        var machineInput = Console.ReadLine()?.Trim().ToUpper();
        Console.CursorVisible = false;

        if (machineInput == "C" || string.IsNullOrEmpty(machineInput)) return;

        if (!int.TryParse(machineInput, out int machineIdx) || machineIdx < 0 || machineIdx >= machinesWithIssues.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Invalid selection.");
            Console.ResetColor();
            Thread.Sleep(1000);
            return;
        }

        var selectedMachine = machinesWithIssues[machineIdx];

        Console.WriteLine();
        Console.WriteLine($"  Bearings on {selectedMachine.Model}:");
        for (int i = 0; i < selectedMachine.Bearings.Length; i++)
        {
            var b = selectedMachine.Bearings[i];
            Console.WriteLine($"  [{i}] {b.Position}: {b.HealthPercentage:F1}% health ({b.GetHealthStatus()})");
        }

        Console.WriteLine();
        Console.CursorVisible = true;
        Console.Write("  Enter bearing index to replace (or 'A' for all, 'C' to cancel): ");
        var bearingInput = Console.ReadLine()?.Trim().ToUpper();
        Console.CursorVisible = false;

        if (bearingInput == "C" || string.IsNullOrEmpty(bearingInput)) return;

        if (bearingInput == "A")
        {
            for (int i = 0; i < selectedMachine.Bearings.Length; i++)
            {
                selectedMachine.ReplaceBearing(i);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ Replaced all {selectedMachine.Bearings.Length} bearings on {selectedMachine.Model}");
            Console.ResetColor();
        }
        else if (int.TryParse(bearingInput, out int bearingIdx))
        {
            if (bearingIdx >= 0 && bearingIdx < selectedMachine.Bearings.Length)
            {
                selectedMachine.ReplaceBearing(bearingIdx);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ Replaced bearing {bearingIdx} ({selectedMachine.Bearings[bearingIdx].Position}) on {selectedMachine.Model}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid bearing index.");
                Console.ResetColor();
            }
        }

        Thread.Sleep(1500);
    }

    private static void MaintenanceScheduler()
    {
        Console.Clear();
        DrawHeader("MAINTENANCE SCHEDULER");
        Console.WriteLine();

        var needingAttention = Floor.GetBearingsNeedingAttention(_warningThreshold).ToList();

        if (!needingAttention.Any())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  No bearings currently require scheduled maintenance.");
            Console.WriteLine("  All systems operating within acceptable parameters.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.WriteLine("  Recommended Maintenance Schedule:");
        Console.WriteLine();

        var critical = needingAttention.Where(x => x.Bearing.RUL >= _criticalThreshold).ToList();
        var warning = needingAttention.Where(x => x.Bearing.RUL >= _warningThreshold && x.Bearing.RUL < _criticalThreshold).ToList();

        if (critical.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  IMMEDIATE ACTION REQUIRED:");
            Console.ResetColor();
            foreach (var item in critical)
            {
                Console.WriteLine($"    • {item.Machine.Model} - Bearing {item.Index} ({item.Bearing.Position})");
                Console.WriteLine($"      Health: {item.Bearing.HealthPercentage:F1}% | Status: {item.Bearing.GetHealthStatus()}");
            }
            Console.WriteLine();
        }

        if (warning.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  SCHEDULE MAINTENANCE:");
            Console.ResetColor();
            foreach (var item in warning)
            {
                Console.WriteLine($"    • {item.Machine.Model} - Bearing {item.Index} ({item.Bearing.Position})");
                Console.WriteLine($"      Health: {item.Bearing.HealthPercentage:F1}% | Status: {item.Bearing.GetHealthStatus()}");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"  Total components requiring attention: {needingAttention.Count}");
        Console.WriteLine($"    Critical: {critical.Count}");
        Console.WriteLine($"    Warning:  {warning.Count}");

        WaitForKey();
    }

    private static void ShowSettings()
    {
        Console.Clear();
        DrawHeader("SETTINGS");
        Console.WriteLine();
        Console.WriteLine($"  Warning Threshold:  {_warningThreshold * 100:F0}% degradation (RUL >= {_warningThreshold})");
        Console.WriteLine($"  Critical Threshold: {_criticalThreshold * 100:F0}% degradation (RUL >= {_criticalThreshold})");
        Console.WriteLine($"  Time Multiplier:    {Floor.TimeMultiplier}x");
        Console.WriteLine();
        Console.WriteLine("  [1] Change Warning Threshold");
        Console.WriteLine("  [2] Change Critical Threshold");
        Console.WriteLine("  [3] Change Time Multiplier");
        Console.WriteLine("  [0] Back");
        Console.WriteLine();
        Console.Write("  Select: ");

        var key = Console.ReadKey(true);

        switch (key.KeyChar)
        {
            case '1':
                Console.WriteLine();
                Console.CursorVisible = true;
                Console.Write("  Enter warning threshold (0.0 - 1.0): ");
                if (double.TryParse(Console.ReadLine(), out double warn) && warn >= 0 && warn <= 1)
                {
                    _warningThreshold = warn;
                }
                Console.CursorVisible = false;
                break;
            case '2':
                Console.WriteLine();
                Console.CursorVisible = true;
                Console.Write("  Enter critical threshold (0.0 - 1.0): ");
                if (double.TryParse(Console.ReadLine(), out double crit) && crit >= 0 && crit <= 1)
                {
                    _criticalThreshold = crit;
                }
                Console.CursorVisible = false;
                break;
            case '3':
                ConfigureTimeMultiplier();
                break;
        }
    }

    private static void ResetProductionFloor()
    {
        Console.Clear();
        DrawHeader("RESET PRODUCTION FLOOR");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  WARNING: This will remove all machines and reset the simulation.");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("  Are you sure? (Y/N): ");

        var key = Console.ReadKey(true);
        if (key.KeyChar == 'y' || key.KeyChar == 'Y')
        {
            Floor.Reset();
            AlertLog.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✓ Production floor has been reset.");
            Console.ResetColor();
            Thread.Sleep(1000);
        }
    }

    private static void ListMachinesCompact()
    {
        Console.WriteLine();
        foreach (var (cnc, index) in Floor.Machines.Select((c, i) => (c, i)))
        {
            var status = cnc.HasFailed ? "FAILED" : cnc.IsOn ? "ON" : "OFF";
            var statusColor = cnc.HasFailed ? ConsoleColor.Red : cnc.IsOn ? ConsoleColor.Green : ConsoleColor.Gray;

            Console.Write($"  [{index}] {cnc.Model,-20} ");
            Console.ForegroundColor = statusColor;
            Console.Write($"{status,-8}");
            Console.ResetColor();
            Console.WriteLine($" ({cnc.Bearings.Length} bearings)");
        }
    }

    private static void DrawHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  ══════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"    {title}");
        Console.WriteLine($"  ══════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
    }

    private static void DrawMenuSection(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  ─── {title} ───");
        Console.ResetColor();
    }

    private static void DrawProgressBar(double percentage, int width)
    {
        int filled = (int)(percentage / 100 * width);
        Console.Write("[");
        Console.Write(new string('█', Math.Min(filled, width)));
        Console.Write(new string('░', Math.Max(0, width - filled)));
        Console.Write("]");
    }

    private static void WaitForKey()
    {
        Console.WriteLine();
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey(true);
    }
}
