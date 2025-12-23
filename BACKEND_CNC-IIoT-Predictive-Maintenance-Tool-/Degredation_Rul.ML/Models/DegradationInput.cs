using Microsoft.ML.Data;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

/// <summary>
/// Shared input model for all 27 degradation prediction models.
/// Each model predicts the degradation trajectory for a specific feature.
/// </summary>
public class DegradationInput
{
    [LoadColumn(2)]
    [ColumnName(@"RMS")]
    public float RMS { get; set; }

    [LoadColumn(3)]
    [ColumnName(@"Peak")]
    public float Peak { get; set; }

    [LoadColumn(4)]
    [ColumnName(@"CrestFactor")]
    public float CrestFactor { get; set; }

    [LoadColumn(5)]
    [ColumnName(@"Kurtosis")]
    public float Kurtosis { get; set; }

    [LoadColumn(6)]
    [ColumnName(@"Skewness")]
    public float Skewness { get; set; }

    [LoadColumn(7)]
    [ColumnName(@"StdDev")]
    public float StdDev { get; set; }

    [LoadColumn(8)]
    [ColumnName(@"PeakToPeak")]
    public float PeakToPeak { get; set; }

    [LoadColumn(9)]
    [ColumnName(@"Mean")]
    public float Mean { get; set; }

    [LoadColumn(10)]
    [ColumnName(@"Variance")]
    public float Variance { get; set; }

    [LoadColumn(11)]
    [ColumnName(@"DominantFreq_Hz")]
    public float DominantFreq_Hz { get; set; }

    [LoadColumn(12)]
    [ColumnName(@"DominantFreq_Mag")]
    public float DominantFreq_Mag { get; set; }

    [LoadColumn(13)]
    [ColumnName(@"Energy_0_500Hz")]
    public float Energy_0_500Hz { get; set; }

    [LoadColumn(14)]
    [ColumnName(@"Energy_500_1000Hz")]
    public float Energy_500_1000Hz { get; set; }

    [LoadColumn(15)]
    [ColumnName(@"Energy_1000_2000Hz")]
    public float Energy_1000_2000Hz { get; set; }

    [LoadColumn(16)]
    [ColumnName(@"Energy_2000_4000Hz")]
    public float Energy_2000_4000Hz { get; set; }

    [LoadColumn(17)]
    [ColumnName(@"Energy_4000_6000Hz")]
    public float Energy_4000_6000Hz { get; set; }

    [LoadColumn(18)]
    [ColumnName(@"Energy_6000_8000Hz")]
    public float Energy_6000_8000Hz { get; set; }

    [LoadColumn(19)]
    [ColumnName(@"Energy_8000_10240Hz")]
    public float Energy_8000_10240Hz { get; set; }

    [LoadColumn(20)]
    [ColumnName(@"Energy_BPFO")]
    public float Energy_BPFO { get; set; }

    [LoadColumn(21)]
    [ColumnName(@"Energy_BPFI")]
    public float Energy_BPFI { get; set; }

    [LoadColumn(22)]
    [ColumnName(@"Energy_BSF")]
    public float Energy_BSF { get; set; }

    [LoadColumn(23)]
    [ColumnName(@"Energy_FTF")]
    public float Energy_FTF { get; set; }

    [LoadColumn(24)]
    [ColumnName(@"Energy_BPFO_2x")]
    public float Energy_BPFO_2x { get; set; }

    [LoadColumn(25)]
    [ColumnName(@"Energy_BPFI_2x")]
    public float Energy_BPFI_2x { get; set; }

    [LoadColumn(26)]
    [ColumnName(@"Energy_BPFO_3x")]
    public float Energy_BPFO_3x { get; set; }

    [LoadColumn(27)]
    [ColumnName(@"Energy_BPFI_3x")]
    public float Energy_BPFI_3x { get; set; }

    [LoadColumn(28)]
    [ColumnName(@"Revolutions")]
    public float Revolutions { get; set; }
}
