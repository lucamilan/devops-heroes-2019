using Microsoft.ML.Data;

namespace mlnet_model
{
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")] public bool Prediction { get; set; }
        
        public string Text { get; set; }
        
        public float Score { get; set; }

        public string Version { get; set; }

        public override string ToString()
        {
            return $"\"{Text}\" Model: '{Version}' Positive: '{Prediction}' Score: {Score}";
        }
    }
}