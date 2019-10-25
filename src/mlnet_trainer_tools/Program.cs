using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML;
using mlnet_trainer;

namespace mlnet_trainer_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var modelFilePath = GetModelFilePath(args.FirstOrDefault());
            
            var dataPath = Path.Combine(Environment.CurrentDirectory, "training-data.tsv");

            var trainer = new TrainerForBinaryClassification(dataPath);

            var metrics = trainer.Evaluate();

            var sb = new StringBuilder();

            sb.AppendLine("Model quality metrics evaluation");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Accuracy: {metrics.Accuracy:P2}");
            sb.AppendLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            sb.AppendLine($"AUCPR: {metrics.AreaUnderPrecisionRecallCurve:P2}");
            sb.AppendLine($"F1Score: {metrics.F1Score:P2}");

            //  Ensure metrics are in a acceptable range
            if (metrics.F1Score < TrainerForBinaryClassification.Threshold)
            {
                throw new ApplicationException("F1Score is too low!");
            }

            //  Save model to disk
            if (trainer.SaveModel(modelFilePath))
            {
                SaveChangeLog(modelFilePath, sb.ToString());

                Console.WriteLine(sb);
                Console.WriteLine($"Data read from: {dataPath}");
                Console.WriteLine($"Model saved on: {modelFilePath}");
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void SaveChangeLog(string modelFilePath, string text)
        {
            var path = Path.Combine( Path.GetDirectoryName(modelFilePath) , "CHANGELOG.txt");
            
            File.WriteAllText(path,text);
        }

        private static string GetModelFilePath(string candidatePath)
        {
#if DEBUG
            return Path.Combine(Environment.CurrentDirectory, "Model.zip");
#else
            if (string.IsNullOrEmpty(candidatePath))
                throw new FileNotFoundException("Model.Zip");
            return candidatePath;
#endif
        }
    }
}