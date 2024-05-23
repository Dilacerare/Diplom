namespace Diplom.BlockchainApp.Entity;

using Microsoft.ML.Data;

public class MiningData
{
    [LoadColumn(0)]
    public float Timestamp { get; set; }

    [LoadColumn(1)]
    public float MiningDifficulty { get; set; }

    [LoadColumn(2)]
    public float NetworkActivity { get; set; }
}