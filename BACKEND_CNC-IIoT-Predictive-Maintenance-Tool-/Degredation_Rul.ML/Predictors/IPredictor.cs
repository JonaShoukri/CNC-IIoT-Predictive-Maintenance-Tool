namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.ML.Predictors;

public interface IPredictor<TInput, TOutput>
{
    string ModelName { get; }
    TOutput Predict(TInput input);
}
