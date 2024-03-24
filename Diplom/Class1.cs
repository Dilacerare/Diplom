using System.Xml;
using Diplom.BlockchainApp.Entity;
using Diplom.MedicalCard.Entity;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Diplom;

internal class Program
{
    static void Main(string[] args)
    {
        List<MedicalRecord> medCards = new List<MedicalRecord>
        {
            new MedicalRecord(
                "Иванов Иван Иванович",
                35,
                "AB+",
                "Пыльца, ацетилсалициловая кислота",
                new string[] { "Аспирин", "Левомицетин" },
                new string[] { "Грипп в 2020 году", "Аллергический дерматит" }
            ),
            
            new MedicalRecord(
                "Петрова Мария Сергеевна",
                45,
                "A-",
                "Пищевые добавки",
                new string[] { "Кальций", "Витамин С" },
                new string[] { "Остеопороз", "Гипертония" }
            ),
            
            new MedicalRecord(
                "Сидоров Николай Александрович",
                60,
                "O+",
                "Пыльца",
                new string[] { "Аллергин", "Кетотифен" },
                new string[] { "Бронхиальная астма", "Пневмония в 2018 году" }
            )
        };

        Blockchain blockchain = new Blockchain(2);

        foreach (var medCard in medCards)
        {
            string data = JsonConvert.SerializeObject(medCard);
            
            Console.WriteLine($"Adding block: {data}");

            Block block = new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
                data, blockchain.GetLatestBlock().Hash);
            blockchain.AddBlock(block);
        }
        
        BlockchainFileManager.SaveBlockchain(blockchain, "C:/Users/aidar/Downloads/blockchain.json");
        
        Blockchain loadedBlockchain = BlockchainFileManager.LoadBlockchain("C:/Users/aidar/Downloads/blockchain.json");
        
        Console.WriteLine($"Checking if blockchain is valid: {blockchain.IsChainValid()}");
        Console.WriteLine($"Checking if loaded blockchain is valid: {loadedBlockchain.IsChainValid()}");
        Console.WriteLine($"Blockchain: {JsonConvert.SerializeObject(blockchain, Formatting.Indented)}");
    }
}
