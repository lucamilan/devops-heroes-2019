using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using mlnet_model;

namespace mlnet_func.Extensibility
{
    public static class MLBuilderExtensions
    {
        public static PredictionEnginePoolBuilder<TData, TPrediction> FromBlob<TData, TPrediction>(
            this PredictionEnginePoolBuilder<TData, TPrediction> builder)
            where TData : class
            where TPrediction : class, new()
        {
            builder.Services.AddSingleton<BlobModelLoader, BlobModelLoader>();
            
            builder.Services.AddOptions<PredictionEnginePoolOptions<SentimentIssue, SentimentPrediction>>()
                .Configure<BlobModelLoader>((options, loader) =>
                {
                    loader.Load().GetAwaiter().GetResult();
                    options.ModelLoader = loader;
                });
            
            return builder;
        }
    }
}