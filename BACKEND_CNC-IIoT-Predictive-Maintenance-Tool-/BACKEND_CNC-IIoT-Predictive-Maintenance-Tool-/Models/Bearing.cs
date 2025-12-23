namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public class Bearing
{
    private static readonly string[] PositionNames = { "Front-Left", "Front-Right", "Rear-Left", "Rear-Right" };

    public int Index { get; }
    public string Position => Index < PositionNames.Length ? PositionNames[Index] : $"Position-{Index}";
    public BearingState State { get; private set; }
    public double RUL => State.RUL;
    public double HealthPercentage => Math.Max(0, (1.0 - RUL) * 100);

    public Bearing(int index)
    {
        Index = index;
        State = BearingInitializer.Instance.CreateInitialState();
    }

    public void UpdateState(BearingState next, double rul)
    {
        State = next with { RUL = rul };
    }

    public string GetHealthStatus()
    {
        return RUL switch
        {
            >= 1.0 => "FAILED",
            >= 0.9 => "CRITICAL",
            >= 0.7 => "WARNING",
            >= 0.5 => "FAIR",
            _ => "GOOD"
        };
    }
}