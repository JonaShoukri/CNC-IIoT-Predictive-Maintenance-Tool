namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public class BearingInitializer
{
    
    public static readonly BearingInitializer Instance = new();
    private static readonly Random _random = new();
    
    private BearingInitializer() { }

    public BearingState CreateInitialState()
    {
        return new BearingState(
            Rms: RandomInRange(Bounds.Rms),
            Peak: RandomInRange(Bounds.Peak),
            CrestFactor: RandomInRange(Bounds.CrestFactor),
            Kurtosis: RandomInRange(Bounds.Kurtosis),
            Skewness: RandomInRange(Bounds.Skewness),
            StdDev: RandomInRange(Bounds.StdDev),
            PeakToPeak: RandomInRange(Bounds.PeakToPeak),
            Mean: RandomInRange(Bounds.Mean),
            Variance: RandomInRange(Bounds.Variance),
            DominantFrequency_Hz: RandomInRange(Bounds.DominantFrequency_Hz),
            DominantFrequency_Mag: RandomInRange(Bounds.DominantFrequency_Mag),
            Energy_0_500Hz: RandomInRange(Bounds.Energy_0_500Hz),
            Energy_500_1000Hz: RandomInRange(Bounds.Energy_500_1000Hz),
            Energy_1000_2000Hz: RandomInRange(Bounds.Energy_1000_2000Hz),
            Energy_2000_4000Hz: RandomInRange(Bounds.Energy_2000_4000Hz),
            Energy_4000_6000Hz: RandomInRange(Bounds.Energy_4000_6000Hz),
            Energy_6000_8000Hz: RandomInRange(Bounds.Energy_6000_8000Hz),
            Energy_8000_10240Hz: RandomInRange(Bounds.Energy_8000_10240Hz),
            EnergyBPFO: RandomInRange(Bounds.EnergyBPFO),
            EnergyBPFI: RandomInRange(Bounds.EnergyBPFI),
            EnergyBSF: RandomInRange(Bounds.EnergyBSF),
            EnergyFTF: RandomInRange(Bounds.EnergyFTF),
            EnergyBPFO_2x: RandomInRange(Bounds.EnergyBPFO_2x),
            EnergyBPFI_2x: RandomInRange(Bounds.EnergyBPFI_2x),
            EnergyBPFO_3x: RandomInRange(Bounds.EnergyBPFO_3x),
            EnergyBPFI_3x: RandomInRange(Bounds.EnergyBPFI_3x),
            Revolutions: 0,
            RUL: 0
        );
    }
    
    private static double RandomInRange((double Min, double Max) bounds)
    {
        return bounds.Min + _random.NextDouble() * (bounds.Max - bounds.Min);
    }
}