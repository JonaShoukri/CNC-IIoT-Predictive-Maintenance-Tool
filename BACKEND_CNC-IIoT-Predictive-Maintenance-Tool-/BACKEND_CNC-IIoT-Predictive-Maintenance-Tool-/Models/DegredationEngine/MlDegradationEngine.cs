using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Predictors;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models.DegredationEngine;

public class MlDegradationEngine : IDegradationEngine
{
    private readonly Dictionary<string, DegradationPredictor> _predictors;
    
    public MlDegradationEngine()
    {
        _predictors = DegradationPredictor.GetAvailableModels()
            .ToDictionary(name => name, name => new DegradationPredictor(name));
    }
    
    public BearingState PredictNext(BearingState current, double revolutionsStep)
    {
        var input = ToInput(current);

        double Predict(string name) =>
            _predictors[name].Predict(input).PredictedValue;

        return current with
        {
            Rms = Predict("RMS"),
            Peak = Predict("Peak"),
            CrestFactor = Predict("CrestFactor"),
            Kurtosis = Predict("Kurtosis"),
            Skewness = Predict("Skewness"),
            StdDev = Predict("StdDev"),
            PeakToPeak = Predict("PeakToPeak"),
            Mean = Predict("Mean"),
            Variance = Predict("Variance"),
            DominantFrequency_Hz = Predict("DominantFreq_Hz"),
            DominantFrequency_Mag = Predict("DominantFreq_Mag"),
            Energy_0_500Hz = Predict("Energy_0_500Hz"),
            Energy_500_1000Hz = Predict("Energy_500_1000Hz"),
            Energy_1000_2000Hz = Predict("Energy_1000_2000Hz"),
            Energy_2000_4000Hz = Predict("Energy_2000_4000Hz"),
            Energy_4000_6000Hz = Predict("Energy_4000_6000Hz"),
            Energy_6000_8000Hz = Predict("Energy_6000_8000Hz"),
            Energy_8000_10240Hz = Predict("Energy_8000_10240Hz"),
            EnergyBPFO = Predict("Energy_BPFO"),
            EnergyBPFI = Predict("Energy_BPFI"),
            EnergyBSF = Predict("Energy_BSF"),
            EnergyFTF = Predict("Energy_FTF"),
            EnergyBPFO_2x = Predict("Energy_BPFO_2x"),
            EnergyBPFI_2x = Predict("Energy_BPFI_2x"),
            EnergyBPFO_3x = Predict("Energy_BPFO_3x"),
            EnergyBPFI_3x = Predict("Energy_BPFI_3x"),
            Revolutions = current.Revolutions + revolutionsStep
        };
    }
    
    private static DegradationInput ToInput(BearingState s) => new()
    {
        RMS = (float)s.Rms,
        Peak = (float)s.Peak,
        CrestFactor = (float)s.CrestFactor,
        Kurtosis = (float)s.Kurtosis,
        Skewness = (float)s.Skewness,
        StdDev = (float)s.StdDev,
        PeakToPeak = (float)s.PeakToPeak,
        Mean = (float)s.Mean,
        Variance = (float)s.Variance,
        DominantFreq_Hz = (float)s.DominantFrequency_Hz,
        DominantFreq_Mag = (float)s.DominantFrequency_Mag,
        Energy_0_500Hz = (float)s.Energy_0_500Hz,
        Energy_500_1000Hz = (float)s.Energy_500_1000Hz,
        Energy_1000_2000Hz = (float)s.Energy_1000_2000Hz,
        Energy_2000_4000Hz = (float)s.Energy_2000_4000Hz,
        Energy_4000_6000Hz = (float)s.Energy_4000_6000Hz,
        Energy_6000_8000Hz = (float)s.Energy_6000_8000Hz,
        Energy_8000_10240Hz = (float)s.Energy_8000_10240Hz,
        Energy_BPFO = (float)s.EnergyBPFO,
        Energy_BPFI = (float)s.EnergyBPFI,
        Energy_BSF = (float)s.EnergyBSF,
        Energy_FTF = (float)s.EnergyFTF,
        Energy_BPFO_2x = (float)s.EnergyBPFO_2x,
        Energy_BPFI_2x = (float)s.EnergyBPFI_2x,
        Energy_BPFO_3x = (float)s.EnergyBPFO_3x,
        Energy_BPFI_3x = (float)s.EnergyBPFI_3x,
        Revolutions = (float)s.Revolutions
    };
}