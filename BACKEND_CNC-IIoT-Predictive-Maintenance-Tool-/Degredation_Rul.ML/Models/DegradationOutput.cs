using Microsoft.ML.Data;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

/// <summary>
/// Shared output model for all 27 degradation prediction models.
/// The Score represents the predicted next value for the target feature.
/// </summary>
public class DegradationOutput
{
    [ColumnName(@"Score")]
    public float PredictedValue { get; set; }
}
