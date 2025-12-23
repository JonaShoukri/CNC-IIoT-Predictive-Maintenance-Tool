using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Predictors;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.RulEngine;

public sealed class MlRulEngine : IRulEngine
{
    private readonly BearingRulPredictor _predictor = new();

    public double PredictRul(BearingState state)
    {
        var input = new BearingRulInput
        {
            RMS = (float)state.Rms,
            Peak = (float)state.Peak,
            CrestFactor = (float)state.CrestFactor,
            Kurtosis = (float)state.Kurtosis,
            Skewness = (float)state.Skewness,
            StdDev = (float)state.StdDev,
            PeakToPeak = (float)state.PeakToPeak,
            Mean = (float)state.Mean,
            Variance = (float)state.Variance,
            DominantFreq_Hz = (float)state.DominantFrequency_Hz,
            DominantFreq_Mag = (float)state.DominantFrequency_Mag,
            Energy_0_500Hz = (float)state.Energy_0_500Hz,
            Energy_500_1000Hz = (float)state.Energy_500_1000Hz,
            Energy_1000_2000Hz = (float)state.Energy_1000_2000Hz,
            Energy_2000_4000Hz = (float)state.Energy_2000_4000Hz,
            Energy_4000_6000Hz = (float)state.Energy_4000_6000Hz,
            Energy_6000_8000Hz = (float)state.Energy_6000_8000Hz,
            Energy_8000_10240Hz = (float)state.Energy_8000_10240Hz,
            Energy_BPFO = (float)state.EnergyBPFO,
            Energy_BPFI = (float)state.EnergyBPFI,
            Energy_BSF = (float)state.EnergyBSF,
            Energy_FTF = (float)state.EnergyFTF,
            Energy_BPFO_2x = (float)state.EnergyBPFO_2x,
            Energy_BPFI_2x = (float)state.EnergyBPFI_2x,
            Energy_BPFO_3x = (float)state.EnergyBPFO_3x,
            Energy_BPFI_3x = (float)state.EnergyBPFI_3x,
            Revolutions = (float)state.Revolutions
        };

        return _predictor.Predict(input).RemainingUsefulLife;
    }
}