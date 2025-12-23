namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public sealed class SimulationClock
{
    public double TimeMultiplier { get; set; } = 1.0;

    private const double SecondsPerIteration = 7.5 * 60;
    private double _accumulatorSeconds;

    public double AccumulatedSeconds => _accumulatorSeconds;
    public double ProgressToNextTick => _accumulatorSeconds / SecondsPerIteration;

    public bool ShouldTick(double realDeltaSeconds)
    {
        _accumulatorSeconds += realDeltaSeconds * TimeMultiplier;

        if (_accumulatorSeconds >= SecondsPerIteration)
        {
            _accumulatorSeconds -= SecondsPerIteration;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _accumulatorSeconds = 0;
        TimeMultiplier = 1.0;
    }
}