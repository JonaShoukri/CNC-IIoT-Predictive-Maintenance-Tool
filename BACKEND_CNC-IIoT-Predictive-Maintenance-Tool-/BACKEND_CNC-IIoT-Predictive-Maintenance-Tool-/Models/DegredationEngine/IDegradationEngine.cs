namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.DegredationEngine;

public interface IDegradationEngine
{
    BearingState PredictNext(BearingState current, double revolutionsStep);
}