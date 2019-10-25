using Microsoft.ML.Data;

namespace mlnet_model
{
    public class SentimentIssue
    {
        /// <summary>
        /// Label: "verità" espressa tramite i valori delle feature associate.
        /// </summary>
        [LoadColumn(0)] public bool Label { get; set; }

        /// <summary>
        /// Feature: Caratteristica del dato che contribuisce a stabilire la verità.
        /// Il motore di ML utilizza le feature per dedurre come arrivare alla verità.
        /// </summary>
        [LoadColumn(1)] public string Text { get; set; }
    }
}