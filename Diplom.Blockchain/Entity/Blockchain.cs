using Newtonsoft.Json;

namespace Diplom.BlockchainApp.Entity;

[Serializable]
public class Blockchain
{
    [JsonProperty]
    internal List<Block> Chain { get; set; } = new List<Block>();

    [JsonProperty]
    internal int Difficulty { get; set; }
    
    private MiningModel miningModel;

    public Blockchain(int difficulty)
    {
        Difficulty = difficulty;
        Chain = new List<Block>();
        AddGenesisBlock();
        
        // Initialize mining model
        miningModel = new MiningModel();
    }

    internal void AddGenesisBlock()
    {
        Chain.Add(new Block(0, DateTime.Now, "GENESIS_BLOCK", ""));
    }

    public Block GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

    public void AddBlock(Block block)
    {
        // Predict optimal difficulty using the model
        Difficulty = (int)miningModel.PredictOptimalDifficulty((float)DateTime.Now.Ticks, (float)Chain.Count);

        block.PreviousHash = GetLatestBlock().Hash;
        block.MineBlock(Difficulty);
        Chain.Add(block);
    }

    public bool IsChainValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block currentBlock = Chain[i];
            Block previousBlock = Chain[i - 1];

            if (currentBlock.Hash != currentBlock.CalculateHash())
            {
                return false;
            }

            if (currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }
        }

        return true;
    }
    public Block GetBlockByHash(string hash)
    {
        return Chain.FirstOrDefault(block => block.Hash == hash);
    }

    public Block GetBlockByIndex(int index)
    {
        return Chain.FirstOrDefault(block => block.Index == index);
    }
}