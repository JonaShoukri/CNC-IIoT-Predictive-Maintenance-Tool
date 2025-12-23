namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.RulEngine;

public interface IRulEngine
{
    double PredictRul(BearingState state);
}