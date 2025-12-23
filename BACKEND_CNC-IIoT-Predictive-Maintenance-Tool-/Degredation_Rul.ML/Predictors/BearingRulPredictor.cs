using Microsoft.ML;
using BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Models;

namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Predictors;

public class BearingRulPredictor : IPredictor<BearingRulInput, BearingRulOutput>
{
    public string ModelName => "BearingRUL";

    private static readonly string ModelPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Resources",
        "BearingRul.mlnet");

    private readonly Lazy<PredictionEngine<BearingRulInput, BearingRulOutput>> _predictionEngine;

    public BearingRulPredictor()
    {
        _predictionEngine = new Lazy<PredictionEngine<BearingRulInput, BearingRulOutput>>(
            CreatePredictionEngine,
            isThreadSafe: true);
    }

    private static PredictionEngine<BearingRulInput, BearingRulOutput> CreatePredictionEngine()
    {
        var mlContext = new MLContext();
        var model = mlContext.Model.Load(ModelPath, out _);
        return mlContext.Model.CreatePredictionEngine<BearingRulInput, BearingRulOutput>(model);
    }

    public BearingRulOutput Predict(BearingRulInput input)
    {
        return _predictionEngine.Value.Predict(input);
    }
}
