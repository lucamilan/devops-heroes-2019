using System;
using System.Linq;
using System.Threading.Tasks;
using mlnet_model;
using RestSharp;

namespace mlnet_func_tester
{
    class Program
    {
        private const string FuncEndpoint = "https://doh2019-ml.azurewebsites.net";

        static void Main(string[] args)
        {
            var count = 1;

            Console.WriteLine("Press space to analyze your sentences...");

            while (Console.ReadKey().Key == ConsoleKey.Spacebar)
            {
                var tasks = Samples.Select(MakePrediction).ToArray();
                var predictions = Task.WhenAll(tasks)
                    .GetAwaiter().GetResult()
                    .Where(_ => _ != null)
                    .ToList();

                Console.Clear();
                Console.WriteLine($"Query counter: {count} result: {predictions.Count}");
                Console.WriteLine();

                foreach (var value in predictions)
                {
                    Console.WriteLine();

                    var defaultColor = Console.ForegroundColor;
                    
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.ForegroundColor = value.Prediction ? ConsoleColor.Green : ConsoleColor.Red;
                    
                    Console.WriteLine( value );

                    Console.ForegroundColor = defaultColor;
                }

                count++;
            }
        }

        private static async Task<SentimentPrediction> MakePrediction(string text)
        {
            var resource = new RestRequest("/api/AnalyzeSentiment");

            resource.AddJsonBody(new
            {
                Text = text
            });

            var response = await new RestClient(FuncEndpoint)
                .ExecutePostTaskAsync<SentimentPrediction>(resource);

            return response.Data;
        }

        private static readonly string[] Samples=
        {
            "This session and that speakers are fantastic",
            "I like it that speech",
            "It's really good speech",
            "It's not so good speech",
            "Recommend! I really enjoyed",
            "Exceptional! liked a lot that speech",
            "This session and that speakers are fantastic",
            "Recommend! I really enjoyed",
            "Waste of time, bad speech",
            "Poor demo! "
        };
    }
}
