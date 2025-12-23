using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.DegredationEngine;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.RulEngine;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public sealed class ProductionFloor
{
    public static readonly ProductionFloor Instance = new();
    private readonly List<CNC> _cncs;
    private readonly IDegradationEngine _engine;
    private readonly IRulEngine _rul;
    private readonly SimulationClock _clock;
    public const double RevolutionsPerIteration = 15000;
    public const double SecondsPerIteration = 7.5 * 60;

    public IReadOnlyList<CNC> Machines => _cncs.AsReadOnly();
    public double TimeMultiplier => _clock.TimeMultiplier;
    public double AccumulatedSeconds => _clock.AccumulatedSeconds;
    public int TotalIterations { get; private set; }
    public bool IsPaused { get; private set; }

    public event Action<CNC, int>? OnBearingFailed;
    public event Action<int>? OnIterationComplete;

    private ProductionFloor()
    {
        _cncs = new();
        _engine = new MlDegradationEngine();
        _rul = new MlRulEngine();
        _clock = new();
        TotalIterations = 0;
        IsPaused = false;
    }

    public void Tick(double realDeltaSeconds)
    {
        if (IsPaused) return;

        if (!_clock.ShouldTick(realDeltaSeconds))
            return;

        TotalIterations++;

        foreach (var cnc in _cncs.Where(c => c.IsOn && !c.HasFailed).ToList())
        {
            bool wasOperational = !cnc.HasFailed;
            cnc.Tick(_engine, _rul, RevolutionsPerIteration);

            if (wasOperational && cnc.HasFailed && cnc.FailedBearingIndex.HasValue)
            {
                OnBearingFailed?.Invoke(cnc, cnc.FailedBearingIndex.Value);
            }
        }

        OnIterationComplete?.Invoke(TotalIterations);
    }

    public void ForceTick()
    {
        TotalIterations++;

        foreach (var cnc in _cncs.Where(c => c.IsOn && !c.HasFailed).ToList())
        {
            bool wasOperational = !cnc.HasFailed;
            cnc.Tick(_engine, _rul, RevolutionsPerIteration);

            if (wasOperational && cnc.HasFailed && cnc.FailedBearingIndex.HasValue)
            {
                OnBearingFailed?.Invoke(cnc, cnc.FailedBearingIndex.Value);
            }
        }

        OnIterationComplete?.Invoke(TotalIterations);
    }

    public void SetTimeMultiplier(double multiplier)
    {
        _clock.TimeMultiplier = Math.Max(0, multiplier);
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;

    public void Reset()
    {
        _cncs.Clear();
        _clock.Reset();
        TotalIterations = 0;
        IsPaused = false;
    }

    public void AddCnc(CNC cnc) => _cncs.Add(cnc);

    public bool RemoveCnc(Guid id)
    {
        var cnc = _cncs.FirstOrDefault(c => c.Id == id);
        if (cnc == null) return false;
        _cncs.Remove(cnc);
        return true;
    }

    public CNC? GetCnc(Guid id) => _cncs.FirstOrDefault(c => c.Id == id);

    public CNC? GetCncByIndex(int index)
    {
        if (index < 0 || index >= _cncs.Count) return null;
        return _cncs[index];
    }

    public int ActiveMachineCount => _cncs.Count(c => c.IsOn && !c.HasFailed);
    public int FailedMachineCount => _cncs.Count(c => c.HasFailed);
    public int TotalBearingCount => _cncs.Sum(c => c.Bearings.Length);

    public IEnumerable<(CNC Machine, Bearing Bearing, int Index)> GetBearingsNeedingAttention(double rulThreshold = 0.7)
    {
        return _cncs
            .Where(c => !c.HasFailed)
            .SelectMany(c => c.Bearings.Select((b, i) => (Machine: c, Bearing: b, Index: i)))
            .Where(x => x.Bearing.RUL >= rulThreshold)
            .OrderByDescending(x => x.Bearing.RUL);
    }
}