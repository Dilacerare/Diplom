using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Diplom.BlockchainApp.Entity;

[Serializable]
public class Block
{
    [JsonProperty]
    public int Index { get; set; }

    [JsonProperty]
    internal DateTime Timestamp { get; set; }

    [JsonProperty]
    public string Hash { get; set; }

    [JsonProperty]
    public string Data { get; set; }

    [JsonProperty]
    internal string PreviousHash { get; set; }

    [JsonProperty]
    internal int Nonce { get; set; }

    public Block(int index, DateTime timestamp, string data, string previousHash)
    {
        Index = index;
        Timestamp = timestamp;
        Data = data;
        PreviousHash = previousHash;
        Hash = CalculateHash();
    }

    internal string CalculateHash()
    {
        SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes($"{Index}-{Timestamp}-{Data}-{PreviousHash}-{Nonce}");
        byte[] outputBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(outputBytes);
    }

    internal void MineBlock(int difficulty)
    {
        string leadingZeros = new string('0', difficulty);

        while (Hash.Substring(0, difficulty) != leadingZeros)
        {
            Nonce++;
            Hash = CalculateHash();
        }
    }
}