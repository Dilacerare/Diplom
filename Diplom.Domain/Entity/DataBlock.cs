namespace Diplom.Domain.Entity;

public class DataBlock
{
    public string Initiator { get; set; }

    public string Addressee { get; set; }

    public string OperationName { get; set; }

    public string EncryptedKey { get; set; }
    
    public string EncryptedIv { get; set; }
    
    public string EncryptedData { get; set; }

    public DataBlock(string initiator, string addressee, string operationName, string encryptedKey, string encryptedIv, string encryptedData)
    {
        Initiator = initiator;
        Addressee = addressee;
        OperationName = operationName;
        EncryptedKey = encryptedKey;
        EncryptedIv = encryptedIv;
        EncryptedData = encryptedData;
    }
}