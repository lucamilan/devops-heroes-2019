using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using mlnet_func;
using mlnet_func.Extensibility;
using mlnet_model;

[assembly: FunctionsStartup(typeof(Startup))]

namespace mlnet_func
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = BuildConfiguration();

            builder.Services.AddSingleton(configuration);

            builder.Services.AddPredictionEnginePool<SentimentIssue, SentimentPrediction>()
                            .FromBlob();
        }

        public IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}