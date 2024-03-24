using Newtonsoft.Json;

namespace Diplom.BlockchainApp.Entity;

public class BlockchainFileManager
{
    public static void SaveBlockchain(Blockchain blockchain, string filePath)
    {
        string jsonBlockchain = JsonConvert.SerializeObject(blockchain, Formatting.Indented);
        File.WriteAllText(filePath, jsonBlockchain);
    }

    public static Blockchain LoadBlockchain(string filePath)
    {
        string jsonBlockchain = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Blockchain>(jsonBlockchain);
    }
}