using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diplom.BlockchainApp.Entity;

public class MiningModel
{
    private MLContext mlContext;
    private PredictionEngine<MiningData, MiningPrediction> predictionEngine;

    public MiningModel()
    {
        mlContext = new MLContext(seed: 0);

        // Load training data
        var trainingData = LoadTrainingData();

        // Define data processing pipeline
        var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "MiningDifficulty")
            .Append(mlContext.Transforms.Concatenate("Features", "Timestamp", "NetworkActivity"))
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

        // Train the model
        var trainingPipeline = pipeline.Fit(trainingData);

        // Create prediction engine
        predictionEngine = mlContext.Model.CreatePredictionEngine<MiningData, MiningPrediction>(trainingPipeline);
    }

    private IDataView LoadTrainingData()
    {
        // Load historical data from your blockchain
        var historicalData = new List<MiningData>
        {
            new MiningData { Timestamp = 1, MiningDifficulty = 3, NetworkActivity = 100 },
            new MiningData { Timestamp = 2, MiningDifficulty = 4, NetworkActivity = 120 },
            // Add more historical data as needed
        };

        // Convert historical data to IDataView
        return mlContext.Data.LoadFromEnumerable(historicalData);
    }

    public float PredictOptimalDifficulty(float currentTimestamp, float networkActivity)
    {
        var input = new MiningData { Timestamp = currentTimestamp, NetworkActivity = networkActivity };
        var prediction = predictionEngine.Predict(input);
        
        float predictedDifficulty = Math.Max(1, Math.Min(2, prediction.PredictedMiningDifficulty));

        return predictedDifficulty;
    }
}

public class MiningPrediction
{
    [ColumnName("Score")]
    public float PredictedMiningDifficulty { get; set; }
}