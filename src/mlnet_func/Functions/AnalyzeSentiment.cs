using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using mlnet_func.Extensibility;
using mlnet_model;
using Newtonsoft.Json;

namespace mlnet_func.Functions
{
    public class AnalyzeSentiment
    {
        private readonly PredictionEnginePool<SentimentIssue, SentimentPrediction> predictionEngine;
        private readonly BlobModelLoader modelLoader;

        public AnalyzeSentiment(PredictionEnginePool<SentimentIssue, SentimentPrediction> predictionEngine, BlobModelLoader modelLoader)
        {
            this.predictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            this.modelLoader = modelLoader;
        }

        [FunctionName(nameof(AnalyzeSentiment))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request with loader {modelLoader.LastModelLoaded}");

            SentimentIssue issue;

            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                issue = JsonConvert.DeserializeObject<SentimentIssue>(body);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }

            var prediction = predictionEngine.Predict(issue);

            prediction.Version = modelLoader.LastModelLoaded;

            return new OkObjectResult(prediction);
        }
    }
}