using Microsoft.ML;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Predictors;

/// <summary>
/// Generic predictor for degradation models. Each model predicts the next value
/// for a specific feature based on current bearing measurements.
/// </summary>
public class DegradationPredictor : IPredictor<DegradationInput, DegradationOutput>
{
    private static readonly string ResourcesPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Resources",
        "Degradation");

    /// <summary>
    /// Available degradation model names (corresponding to trained .mlnet files)
    /// </summary>
    public static readonly string[] AvailableModels = new[]
    {
        "RMS", "Peak", "CrestFactor", "Kurtosis", "Skewness", "StdDev", "PeakToPeak",
        "Mean", "Variance", "DominantFreq_Hz", "DominantFreq_Mag",
        "Energy_0_500Hz", "Energy_500_1000Hz", "Energy_1000_2000Hz", "Energy_2000_4000Hz",
        "Energy_4000_6000Hz", "Energy_6000_8000Hz", "Energy_8000_10240Hz",
        "Energy_BPFO", "Energy_BPFI", "Energy_BSF", "Energy_FTF",
        "Energy_BPFO_2x", "Energy_BPFI_2x", "Energy_BPFO_3x", "Energy_BPFI_3x"
    };

    private readonly string _modelName;
    private readonly Lazy<PredictionEngine<DegradationInput, DegradationOutput>> _predictionEngine;

    public string ModelName => _modelName;

    /// <summary>
    /// Creates a degradation predictor for the specified feature.
    /// </summary>
    /// <param name="modelName">The feature name (e.g., "RMS", "Peak", "Kurtosis")</param>
    /// <exception cref="ArgumentException">Thrown when model name is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when model file doesn't exist</exception>
    public DegradationPredictor(string modelName)
    {
        if (!AvailableModels.Contains(modelName))
        {
            throw new ArgumentException(
                $"Invalid model name: {modelName}. Available models: {string.Join(", ", AvailableModels)}",
                nameof(modelName));
        }

        _modelName = modelName;
        _predictionEngine = new Lazy<PredictionEngine<DegradationInput, DegradationOutput>>(
            CreatePredictionEngine,
            isThreadSafe: true);
    }

    private PredictionEngine<DegradationInput, DegradationOutput> CreatePredictionEngine()
    {
        var modelPath = Path.Combine(ResourcesPath, $"{_modelName}.mlnet");

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException(
                $"Model file not found: {modelPath}. Ensure the model has been trained.",
                modelPath);
        }

        var mlContext = new MLContext();
        var model = mlContext.Model.Load(modelPath, out _);
        return mlContext.Model.CreatePredictionEngine<DegradationInput, DegradationOutput>(model);
    }

    public DegradationOutput Predict(DegradationInput input)
    {
        return _predictionEngine.Value.Predict(input);
    }

    /// <summary>
    /// Checks if a specific model file exists and is ready for use.
    /// </summary>
    public static bool IsModelAvailable(string modelName)
    {
        var modelPath = Path.Combine(ResourcesPath, $"{modelName}.mlnet");
        return File.Exists(modelPath);
    }

    /// <summary>
    /// Returns a list of all models that are currently available (have trained .mlnet files).
    /// </summary>
    public static IEnumerable<string> GetAvailableModels()
    {
        return AvailableModels.Where(IsModelAvailable);
    }
}
