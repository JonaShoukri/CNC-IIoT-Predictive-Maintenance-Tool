using Microsoft.ML.Data;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

public class BearingRulOutput
{
    [ColumnName(@"Score")]
    public float RemainingUsefulLife { get; set; }
}
