using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.DegredationEngine;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.RulEngine;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public class CNC
{
    private static readonly string[] ModelNames =
    {
        "Haas VF-2", "DMG MORI DMU 50", "Mazak Integrex i-200",
        "Okuma GENOS M560-V", "Fanuc RoboDrill", "Hurco VMX42i",
        "Doosan DNM 500", "Brother Speedio", "Makino a51nx"
    };

    public Guid Id { get; }
    public string Model { get; }
    public Bearing[] Bearings { get; }
    public bool IsOn { get; private set; }
    public bool HasFailed { get; private set; }
    public int? FailedBearingIndex { get; private set; }

    public CNC(string? modelName = null)
    {
        Id = Guid.NewGuid();
        Model = modelName ?? ModelNames[Random.Shared.Next(ModelNames.Length)];
        IsOn = false;

        int count = Random.Shared.Next(2) == 0 ? 2 : 4;
        Bearings = new Bearing[count];

        for (int i = 0; i < count; i++)
        {
            Bearings[i] = new Bearing(i);
        }
        HasFailed = false;
        FailedBearingIndex = null;
    }

    public bool TurnOn()
    {
        if (HasFailed) return false;
        IsOn = true;
        return true;
    }

    public void TurnOff()
    {
        IsOn = false;
    }

    public void TogglePower()
    {
        if (IsOn)
            TurnOff();
        else
            TurnOn();
    }
    
    public void Tick(IDegradationEngine degradation, IRulEngine rulEngine, double revolutionsStep)
    {
        for (int i = 0; i < Bearings.Length; i++)
        {
            var bearing = Bearings[i];
            var degraded = degradation.PredictNext(bearing.State, revolutionsStep);
            var rul = rulEngine.PredictRul(degraded);

            bearing.UpdateState(degraded, rul);

            if (FailureCriteria.IsFailed(rul))
            {
                HasFailed = true;
                IsOn = false;
                FailedBearingIndex = i;
                return;
            }
        }
    }

    public void ReplaceBearing(int index)
    {
        if (index < 0 || index >= Bearings.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        Bearings[index] = new Bearing(index);

        if (FailedBearingIndex == index)
        {
            HasFailed = false;
            FailedBearingIndex = null;
        }
    }

    public double GetLowestRul()
    {
        return Bearings.Min(b => 1.0 - b.RUL);
    }

    public Bearing GetMostDegradedBearing()
    {
        return Bearings.MaxBy(b => b.RUL)!;
    }
}