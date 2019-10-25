using System;
using System.IO;
using FluentAssertions;
using Microsoft.ML;
using mlnet_model;
using mlnet_trainer;
using mlnet_trainer_tests.Scenario_01;
using Xunit;
using Xunit.Abstractions;

namespace mlnet_trainer_tests
{
    public class BinaryClassificationFixture
    {
        private readonly ITestOutputHelper testOutputHelper;

        public BinaryClassificationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            ModelFilePath= Path.Combine(Environment.CurrentDirectory, "Model.zip");
            DataFilePath = Path.Combine(Environment.CurrentDirectory, "training-data.tsv");
            Sut = new TrainerForBinaryClassification(DataFilePath);
        }

        public string ModelFilePath { get; set; }
        public string DataFilePath { get; set; }
        public TrainerForBinaryClassification Sut { get; set; }

        [Theory]
        [MemberData(nameof(FeedbackScenario.Inputs), MemberType = typeof(FeedbackScenario))]
        public void should_verify_prediction(string issue, bool expected)
        {
            // Act
            var sampleStatement = new SentimentIssue
            {
                Text = issue
            };

            //  Arrange
            var result = Sut.Predict(sampleStatement);

            //  Assert
            result.Prediction.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(FeedbackScenario.Inputs), MemberType = typeof(FeedbackScenario))]
        public void should_verify_prediction_from_serialized_model(string issue, bool expected)
        {
            // Act
            var context = new MLContext();
            var sampleStatement = new SentimentIssue
            {
                Text = issue
            };

            //  Arrange

            Sut.SaveModel(ModelFilePath);
            var transformer = context.Model.Load(ModelFilePath, out _);
            var predictionEngine = context.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(transformer);
            var result = predictionEngine.Predict(sampleStatement);
            
            testOutputHelper.WriteLine(result.ToString());

            //  Assert
            result.Prediction.Should().Be(expected);
        }

        [Fact]
        public void should_metrics_be_in_range()
        {
            // Act

            //  Arrange
            var metrics = Sut.Evaluate();

            //  Assert
            metrics.Accuracy.Should().BeInRange(TrainerForBinaryClassification.Threshold,0.99f);
            metrics.AreaUnderRocCurve.Should().BeInRange(TrainerForBinaryClassification.Threshold,1);
            metrics.AreaUnderPrecisionRecallCurve.Should().BeInRange(TrainerForBinaryClassification.Threshold,1);
            metrics.F1Score.Should().BeInRange(TrainerForBinaryClassification.Threshold,1);
        }
    }
}