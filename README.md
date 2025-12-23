<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
  <img src="https://img.shields.io/badge/ML.NET-3.0-00BCF2?style=for-the-badge&logo=microsoft&logoColor=white" alt="ML.NET"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License"/>
  <img src="https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey?style=for-the-badge" alt="Platform"/>
</p>

<h1 align="center">
  CNC Production Floor Digital Twin
</h1>

<h3 align="center">
  IIoT Predictive Maintenance Simulation Platform
</h3>

<p align="center">
  <b>Simulate. Predict. Prevent.</b>
  <br>
  A real-time digital twin for CNC machine ball bearing degradation using ML.NET predictive models
</p>

---

## Why This Matters

Bearing failures in CNC spindles cost **$1,500–$40,000** per incident plus 4–72 hours of unplanned downtime. In aerospace manufacturing (titanium, Inconel machining), a single failed spindle bearing can cascade into:

- Missed delivery deadlines on flight-critical components
- Scrap costs from mid-cut failures
- OEE drops of 15–30% during recovery

This tool demonstrates how **predictive maintenance** shifts from reactive ("fix it when it breaks") to proactive scheduling — the foundation of world-class operations.

## Overview

This project is a **digital twin simulation** of a CNC production floor that models ball bearing degradation over time using machine learning. It enables production floor managers to:

- **Simulate** realistic bearing wear patterns based on NASA's IMS bearing dataset (https://data.nasa.gov/dataset/ims-bearings)
- **Predict** remaining useful life (RUL) before failure occurs
- **Schedule** proactive maintenance to prevent unplanned downtime
- **Monitor** multiple CNC machines simultaneously in real-time

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                         LIVE SIMULATION DASHBOARD                               │
├─────────────────────────────────────────────────────────────────────────────────┤
│  Iteration: 156  |  Simulated Time: 1d 19h 30m  |  Speed: 450x                  │
│  Machines: 4 active | 1 off | 1 failed                                          │
├─────────────────────────────────────────────────────────────────────────────────┤
│  MACHINE                  │ STATUS   │ BEARING HEALTH                           │
│  Haas VF-2                │ RUNNING  │ B0:████████ B1:██████░░ B2:████░░░░       │
│  DMG MORI DMU 50          │ RUNNING  │ B0:████████ B1:██████░░                   │
│  Mazak Integrex i-200     │ FAILED   │ B0:████████ B1:░░░░░░░░ [FAILED]          │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ⚠ CRITICAL: Haas VF-2 - Rear-Left at 12% health                                │
│  ⚠ WARNING:  DMG MORI DMU 50 - Front-Right at 35% health                        │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## Key Features

### Real-Time Digital Twin Simulation

| Feature | Description |
|---------|-------------|
| **Multi-Machine Support** | Manage unlimited CNC machines on a virtual production floor |
| **Realistic Degradation** | 26 ML models predict feature evolution based on real bearing data |
| **RUL Prediction** | Dedicated model estimates remaining useful life from 28 vibration features |
| **Time Acceleration** | Run simulations from 1x (real-time) up to 4500x speed |
| **Live Dashboard** | Watch bearing health degrade in real-time with visual progress bars |

### Machine Learning Pipeline

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│  Current State   │────▶│  26 Degradation  │────▶│   Next State     │
│  (28 features)   │     │     Models       │     │   Prediction     │
└──────────────────┘     └──────────────────┘     └────────┬─────────┘
                                                           │
                                                           ▼
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│  Failure Alert   │◀────│    RUL Model     │◀────│  Degraded State  │
│  (RUL >= 1.0)    │     │   (Regression)   │     │  (28 features)   │
└──────────────────┘     └──────────────────┘     └──────────────────┘
```

### Vibration Feature Analysis

The simulation tracks **28 bearing health indicators** derived from vibration analysis:

| Category | Features |
|----------|----------|
| **Statistical** | RMS, Peak, Crest Factor, Kurtosis, Skewness, Std Dev, Peak-to-Peak, Mean, Variance |
| **Frequency Domain** | Dominant Frequency (Hz), Dominant Frequency Magnitude |
| **Energy Bands** | 0-500Hz, 500-1kHz, 1-2kHz, 2-4kHz, 4-6kHz, 6-8kHz, 8-10.24kHz |
| **Fault Frequencies** | BPFO, BPFI, BSF, FTF (+ 2x and 3x harmonics for BPFO/BPFI) |
| **Operational** | Revolutions, RUL (Remaining Useful Life) |

---

## Architecture

```
CNC-IIoT-Predictive-Maintenance-Tool/
│
├── BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-/
│   │
│   ├── BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-/    # Main Console Application
│   │   ├── Models/
│   │   │   ├── ProductionFloor.cs      # Singleton floor manager
│   │   │   ├── CNC.cs                  # CNC machine entity
│   │   │   ├── Bearing.cs              # Bearing component
│   │   │   ├── BearingState.cs         # Immutable state record (28 features)
│   │   │   ├── SimulationClock.cs      # Time management
│   │   │   ├── FailureCriteria.cs      # Failure thresholds
│   │   │   │
│   │   │   ├── DegredationEngine/
│   │   │   │   ├── IDegradationEngine.cs
│   │   │   │   └── MlDegradationEngine.cs   # 26 ML model orchestrator
│   │   │   │
│   │   │   ├── RulEngine/
│   │   │   │   ├── IRulEngine.cs
│   │   │   │   └── MlRulEngine.cs           # RUL prediction engine
│   │   │   │
│   │   │   └── Bearing Initilizer/
│   │   │       ├── BearingInitializer.cs    # Realistic state generator
│   │   │       └── Bounds.cs                # Feature value ranges
│   │   │
│   │   └── Program.cs                  # Interactive console UI
│   │
│   ├── Degredation_Rul.ML/             # ML.NET Library
│   │   ├── Models/
│   │   │   ├── DegradationInput.cs     # ML input schema
│   │   │   ├── DegradationOutput.cs    # ML output schema
│   │   │   ├── BearingRulInput.cs      # RUL input schema
│   │   │   └── BearingRulOutput.cs     # RUL output schema
│   │   │
│   │   ├── Predictors/
│   │   │   ├── IPredictor.cs           # Generic predictor interface
│   │   │   ├── DegradationPredictor.cs # Feature degradation predictor
│   │   │   └── BearingRulPredictor.cs  # RUL predictor
│   │   │
│   │   └── Resources/
│   │       ├── BearingRul.mlnet        # Trained RUL model
│   │       └── Degradation/            # 26 trained degradation models
│   │           ├── RMS.mlnet
│   │           ├── Peak.mlnet
│   │           ├── Kurtosis.mlnet
│   │           └── ... (23 more)
│   │
│   └── DATA/                           # Training Data
│       ├── raw/                        # NASA IMS bearing dataset
│       │   ├── 1st_test/
│       │   ├── 2nd_test/
│       │   └── 3rd_test/
│       └── treated/                    # Processed training data
│
└── README.md
```

---

## How It Works

### Simulation Cycle

Every **7.5 simulated minutes** (configurable speed), the following occurs:

```
1. DEGRADATION PHASE
   ┌─────────────────────────────────────────────────────────────────┐
   │ For each ACTIVE CNC machine:                                    │
   │   For each bearing:                                             │
   │     • Current state → 26 ML models → Predicted next state       │
   │     • Add 15,000 revolutions to operational counter             │
   └─────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
2. RUL PREDICTION PHASE
   ┌─────────────────────────────────────────────────────────────────┐
   │ For each degraded bearing:                                      │
   │   • Degraded state → RUL model → Remaining useful life value    │
   │   • RUL range: 0.0 (new) → 1.0 (failed)                        │
   └─────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
3. FAILURE CHECK PHASE
   ┌─────────────────────────────────────────────────────────────────┐
   │ If any bearing RUL >= 1.0:                                      │
   │   • Mark bearing as FAILED                                      │
   │   • Shut down CNC machine                                       │
   │   • Log failure event                                           │
   │   • Await maintenance (bearing replacement)                     │
   └─────────────────────────────────────────────────────────────────┘
```

### Health Status Classification

| RUL Value | Status | Health % | Action |
|-----------|--------|----------|--------|
| 0.0 - 0.5 | GOOD | 50-100% | Normal operation |
| 0.5 - 0.7 | FAIR | 30-50% | Monitor closely |
| 0.7 - 0.9 | WARNING | 10-30% | Schedule maintenance |
| 0.9 - 1.0 | CRITICAL | 0-10% | Immediate action required |
| >= 1.0 | FAILED | 0% | Machine shutdown |

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Terminal with Unicode support (for progress bars and symbols)

### Installation

```bash
# Clone the repository
git clone https://github.com/JonaShoukri/CNC-IIoT-Predictive-Maintenance-Tool.git

# Navigate to the project
cd CNC-IIoT-Predictive-Maintenance-Tool/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-

# Build the project
dotnet build

# Run the simulation
dotnet run
```

### Quick Start Guide

1. **Add Machines** - Press `5` to add CNC machines to your floor
2. **Power On** - Press `7` then `A` to turn all machines on
3. **Run Simulation** - Press `A` to run accelerated simulation
4. **Watch** - Observe bearings degrade in real-time on the live dashboard
5. **Maintain** - Press `D` to replace failing bearings before they break

---

## Console Interface

### Main Menu

```
══════════════════════════════════════════════════════════════════════════
  MAIN CONTROL PANEL
══════════════════════════════════════════════════════════════════════════
  Machines: 5 total | 4 active | 0 off | 1 failed
  Iteration: 234 | Time Multiplier: 1x

  ⚠ 3 bearing(s) need attention!

  ─── DASHBOARD ───
  [1] Production Floor Overview
  [2] Machine Status Dashboard
  [3] Bearing Health Monitor
  [4] Alert Log

  ─── MACHINE MANAGEMENT ───
  [5] Add New CNC Machine
  [6] Remove CNC Machine
  [7] Toggle Machine Power
  [8] Inspect Machine Details

  ─── SIMULATION ───
  [9] Run Real-Time Simulation
  [A] Run Accelerated Simulation
  [B] Step Simulation (Single Tick)
  [C] Configure Time Multiplier

  ─── MAINTENANCE ───
  [D] Replace Bearing
  [E] Maintenance Scheduler

  ─── SYSTEM ───
  [S] Settings
  [R] Reset Production Floor
  [Q] Quit
```

### Simulation Speeds

| Multiplier | Real Time per Iteration | Use Case |
|------------|------------------------|----------|
| 1x | 7.5 minutes | Real-time monitoring |
| 10x | 45 seconds | Casual observation |
| 60x | 7.5 seconds | Quick testing |
| 450x | 1 second | Rapid simulation |
| 4500x | 0.1 seconds | Stress testing |

---

## Machine Learning Models

### Training Data

Models are trained on the **NASA IMS Bearing Dataset** - a run-to-failure experiment with:
- 4 bearings under constant load
- 20 kHz sampling rate
- ~35 days of continuous operation until failure
- Real vibration signatures from healthy to degraded to failed states

### Model Architecture

| Model Type | Algorithm | Purpose |
|------------|-----------|---------|
| Degradation (x26) | FastTree Regression | Predict next value for each feature |
| RUL | FastTree Regression | Estimate remaining useful life |

### Feature Importance

The most predictive features for bearing failure:

1. **Kurtosis** - Spikiness of vibration signal (early fault indicator)
2. **RMS** - Overall vibration energy level
3. **BPFO/BPFI Energy** - Ball pass frequencies (defect signatures)
4. **Crest Factor** - Peak-to-RMS ratio
5. **High-frequency Energy** - 6-10 kHz bands

---

## Supported CNC Models

The simulation includes realistic CNC machine models:

| Model | Manufacturer | Bearings |
|-------|--------------|----------|
| VF-2 | Haas | 2 or 4 |
| DMU 50 | DMG MORI | 2 or 4 |
| Integrex i-200 | Mazak | 2 or 4 |
| GENOS M560-V | Okuma | 2 or 4 |
| RoboDrill | Fanuc | 2 or 4 |
| VMX42i | Hurco | 2 or 4 |
| DNM 500 | Doosan | 2 or 4 |
| Speedio | Brother | 2 or 4 |
| a51nx | Makino | 2 or 4 |

---

## API Reference

### ProductionFloor (Singleton)

```csharp
// Access the production floor
var floor = ProductionFloor.Instance;

// Add/remove machines
floor.AddCnc(new CNC("Custom Model"));
floor.RemoveCnc(machineId);

// Control simulation
floor.Tick(deltaSeconds);        // Advance simulation
floor.ForceTick();               // Force single iteration
floor.SetTimeMultiplier(450);    // Set speed

// Query state
floor.Machines                   // IReadOnlyList<CNC>
floor.ActiveMachineCount         // Running machines
floor.FailedMachineCount         // Failed machines
floor.TotalIterations            // Simulation iterations
floor.GetBearingsNeedingAttention(0.7)  // Get warnings

// Events
floor.OnBearingFailed += (cnc, bearingIndex) => { };
floor.OnIterationComplete += (iteration) => { };
```

### CNC Machine

```csharp
var cnc = new CNC("Haas VF-2");

cnc.Id                    // Guid
cnc.Model                 // string
cnc.Bearings              // Bearing[]
cnc.IsOn                  // bool
cnc.HasFailed             // bool
cnc.FailedBearingIndex    // int?

cnc.TurnOn();             // Returns false if failed
cnc.TurnOff();
cnc.ReplaceBearing(index);
cnc.GetMostDegradedBearing();
```

### Bearing

```csharp
bearing.Index             // Position index (0-3)
bearing.Position          // "Front-Left", "Front-Right", etc.
bearing.State             // BearingState record
bearing.RUL               // 0.0 (new) to 1.0 (failed)
bearing.HealthPercentage  // 100% (new) to 0% (failed)
bearing.GetHealthStatus() // "GOOD", "FAIR", "WARNING", "CRITICAL", "FAILED"
```

---

## Future Roadmap

- [ ] **REST API** - ASP.NET Core API for external integrations
- [ ] **Web Dashboard** - Real-time browser-based monitoring
- [ ] **Database Persistence** - Save/load simulation state
- [ ] **Custom ML Training** - Train models on your own bearing data
- [ ] **IoT Integration** - Connect real sensors via MQTT
- [ ] **Notification System** - Email/SMS alerts for critical failures
- [ ] **Cost Analysis** - Track downtime costs and maintenance ROI

---

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- **NASA IMS Bearing Dataset** - Training data from the Center for Intelligent Maintenance Systems
- **ML.NET** - Microsoft's machine learning framework for .NET
- **FastTree** - Gradient boosting decision tree algorithm

---

<p align="center">
  <b>Built for Industry 4.0</b>
  <br>
  Predictive Maintenance | Digital Twin | Machine Learning
</p>
