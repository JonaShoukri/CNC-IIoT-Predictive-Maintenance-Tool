namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

    public static class Bounds
    {
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