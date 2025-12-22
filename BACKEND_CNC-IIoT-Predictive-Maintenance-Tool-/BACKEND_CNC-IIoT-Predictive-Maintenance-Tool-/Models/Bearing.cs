namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public class Bearing
{
    private static readonly Random _random = new();

    public double Rms { get; set; }
    public double Peak { get; set; }
    public double CrestFactor { get; set; }
    public double Kurtosis { get; set; }
    public double Skewness { get; set; }
    public double StdDev { get; set; }
    public double PeakToPeak { get; set; }
    public double Mean { get; set; }
    public double Variance { get; set; }
    public double DominantFrequency_Hz { get; set; }
    public double DominantFrequency_Mag { get; set; }
    public double Energy_0_500Hz { get; set; }
    public double Energy_500_1000Hz { get; set; }
    public double Energy_1000_2000Hz { get; set; }
    public double Energy_2000_4000Hz { get; set; }
    public double Energy_4000_6000Hz { get; set; }
    public double Energy_6000_8000Hz { get; set; }
    public double Energy_8000_10240Hz { get; set; }
    public double EnergyBPFO { get; set; }
    public double EnergyBPFI { get; set; }
    public double EnergyBSF { get; set; }
    public double EnergyFTF { get; set; }
    public double EnergyBPFO_2x { get; set; }
    public double EnergyBPFI_2x { get; set; }
    public double EnergyBPFO_3x { get; set; }
    public double EnergyBPFI_3x { get; set; }
    public double Revolutions { get; set; }
    
    private static class Bounds
    {
        // Format: (Min, Max) - UPDATE THESE WITH YOUR ACTUAL DATA
        public static readonly (double Min, double Max) Rms = ( 0.0541, 0.1318);
        public static readonly (double Min, double Max) Peak = (0.264, 1.023);
        public static readonly (double Min, double Max) CrestFactor = (3.788, 9.361);
        public static readonly (double Min, double Max) Kurtosis = (0.0659, 3.2130);
        public static readonly (double Min, double Max) Skewness = (-0.920, 0.2048);
        public static readonly (double Min, double Max) StdDev = (0.052, 0.109);
        public static readonly (double Min, double Max) PeakToPeak = (0.457, 1.934);
        public static readonly (double Min, double Max) Mean = (-0.0940, -0.0039);
        public static readonly (double Min, double Max) Variance = (0.003, 0.012);
        public static readonly (double Min, double Max) DominantFrequency_Hz = (0.625, 1008.75);
        public static readonly (double Min, double Max) DominantFrequency_Mag = (0.00862, 0.0725);
        public static readonly (double Min, double Max) Energy_0_500Hz = (0.0208, 0.0940);
        public static readonly (double Min, double Max) Energy_500_1000Hz = (0.0147, 0.0322);
        public static readonly (double Min, double Max) Energy_1000_2000Hz = (0.020, 0.0623);
        public static readonly (double Min, double Max) Energy_2000_4000Hz = (0.0248, 0.0494);
        public static readonly (double Min, double Max) Energy_4000_6000Hz = (0.0234, 0.0709);
        public static readonly (double Min, double Max) Energy_6000_8000Hz = (0.0123, 0.0521);
        public static readonly (double Min, double Max) Energy_8000_10240Hz = (0.01086, 0.0536);
        public static readonly (double Min, double Max) EnergyBPFO = (0.0025, 0.01596);
        public static readonly (double Min, double Max) EnergyBPFI = (0.00137, 0.0041);
        public static readonly (double Min, double Max) EnergyBSF = (0.0012, 0.0039);
        public static readonly (double Min, double Max) EnergyFTF = (0.0011, 0.0046);
        public static readonly (double Min, double Max) EnergyBPFO_2x = (0.0026, 0.009);
        public static readonly (double Min, double Max) EnergyBPFI_2x = (0.0017, 0.0048);
        public static readonly (double Min, double Max) EnergyBPFO_3x = (0.0012, 0.0044);
        public static readonly (double Min, double Max) EnergyBPFI_3x = (0.0019, 0.0049);
    }

    public Bearing()
    {
        Rms = RandomInRange(Bounds.Rms);
        Peak = RandomInRange(Bounds.Peak);
        CrestFactor = RandomInRange(Bounds.CrestFactor);
        Kurtosis = RandomInRange(Bounds.Kurtosis);
        Skewness = RandomInRange(Bounds.Skewness);
        StdDev = RandomInRange(Bounds.StdDev);
        PeakToPeak = RandomInRange(Bounds.PeakToPeak);
        Mean = RandomInRange(Bounds.Mean);
        Variance = RandomInRange(Bounds.Variance);
        DominantFrequency_Hz = RandomInRange(Bounds.DominantFrequency_Hz);
        DominantFrequency_Mag = RandomInRange(Bounds.DominantFrequency_Mag);
        Energy_0_500Hz = RandomInRange(Bounds.Energy_0_500Hz);
        Energy_500_1000Hz = RandomInRange(Bounds.Energy_500_1000Hz);
        Energy_1000_2000Hz = RandomInRange(Bounds.Energy_1000_2000Hz);
        Energy_2000_4000Hz = RandomInRange(Bounds.Energy_2000_4000Hz);
        Energy_4000_6000Hz = RandomInRange(Bounds.Energy_4000_6000Hz);
        Energy_6000_8000Hz = RandomInRange(Bounds.Energy_6000_8000Hz);
        Energy_8000_10240Hz = RandomInRange(Bounds.Energy_8000_10240Hz);
        EnergyBPFO = RandomInRange(Bounds.EnergyBPFO);
        EnergyBPFI = RandomInRange(Bounds.EnergyBPFI);
        EnergyBSF = RandomInRange(Bounds.EnergyBSF);
        EnergyFTF = RandomInRange(Bounds.EnergyFTF);
        EnergyBPFO_2x = RandomInRange(Bounds.EnergyBPFO_2x);
        EnergyBPFI_2x = RandomInRange(Bounds.EnergyBPFI_2x);
        EnergyBPFO_3x = RandomInRange(Bounds.EnergyBPFO_3x);
        EnergyBPFI_3x = RandomInRange(Bounds.EnergyBPFI_3x);
        Revolutions = 0; 
    }
    
    private static double RandomInRange((double Min, double Max) bounds)
    {
        return bounds.Min + _random.NextDouble() * (bounds.Max - bounds.Min);
    }

    new public string ToString()
    {
        return $"Bearing [Rms={Rms}, Peak={Peak}, CrestFactor={CrestFactor}, Kurtosis={Kurtosis}, Skewness={Skewness}, StdDev={StdDev}, PeakToPeak={PeakToPeak}, Mean={Mean}, Variance={Variance}, DominantFrequency_Hz={DominantFrequency_Hz}, DominantFrequency_Mag={DominantFrequency_Mag}, Energy_0_500Hz={Energy_0_500Hz}, Energy_500_1000Hz={Energy_500_1000Hz}, Energy_1000_2000Hz={Energy_1000_2000Hz}, Energy_2000_4000Hz={Energy_2000_4000Hz}, Energy_4000_6000Hz={Energy_4000_6000Hz}, Energy_6000_8000Hz={Energy_6000_8000Hz}, Energy_8000_10240Hz={Energy_8000_10240Hz}, EnergyBPFO={EnergyBPFO}, EnergyBPFI={EnergyBPFI}, EnergyBSF={EnergyBSF}, EnergyFTF={EnergyFTF}, EnergyBPFO_2x={EnergyBPFO_2x}, EnergyBPFI_2x={EnergyBPFI_2x}, EnergyBPFO_3x={EnergyBPFO_3x}, EnergyBPFI_3x={EnergyBPFI_3x}, Revolutions={Revolutions}]";
    }
}