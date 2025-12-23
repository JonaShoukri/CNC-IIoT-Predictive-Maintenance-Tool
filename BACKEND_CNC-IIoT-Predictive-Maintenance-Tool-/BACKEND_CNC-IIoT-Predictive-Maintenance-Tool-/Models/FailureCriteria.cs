namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public static class FailureCriteria
{
    public const double BearingFailureRul = .92;

    public static bool IsFailed(double rul)
        => rul >= BearingFailureRul;
}