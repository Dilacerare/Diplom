using System.Xml;
using Diplom.BlockchainApp.Entity;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System.Security.Cryptography;
using System.Text;
using Diplom.Domain.Entity;

namespace Diplom;

internal class Program
{
    static void Main(string[] args)
    {
        // try
        // {
        //     string publicKey;
        //     string privateKey;
        //     using (var rsa = new RSACryptoServiceProvider(2048))
        //     {
        //         publicKey = rsa.ToXmlString(false);
        //         privateKey = rsa.ToXmlString(true);
        //     }
        //
        //     // Пример использования методов
        //     string originalData = "{\"Id\":\"1\",\"PatientName\":\"Иванов Иван Иванович\",\"Age\":35,\"BloodType\":\"AB+\",\"Allergies\":\"Пыльца, ацетилсалициловая кислота\",\"Medications\":[\"Аспирин\",\"Левомицетин\"],\"MedicalHistory\":[\"Грипп в 2020 году\",\"Аллергический дерматит\"]}";
        //     // string originalData = "Hello word!";
        //     string encryptedData;
        //     string encryptedKey;
        //     string encryptedIv;
        //
        //     Encrypt(originalData, publicKey, out encryptedData, out encryptedKey, out encryptedIv);
        //
        //     string decryptedData = Decrypt(encryptedData, privateKey, encryptedKey, encryptedIv);
        //     
        //     Console.WriteLine("Encrypted key:");
        //     Console.WriteLine(encryptedKey);
        //     
        //     Console.WriteLine("Encrypted data:");
        //     Console.WriteLine(encryptedData);
        //     
        //     Console.WriteLine("Decrypted data:");
        //     Console.WriteLine(decryptedData);
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"An error occurred: {ex.Message}");
        // }
        
        List<MedicalRecord> medCards = new List<MedicalRecord>
        {
            new MedicalRecord(
                1,
                "Иванов Иван Иванович",
                35,
                "AB+",
                "Пыльца, ацетилсалициловая кислота",
                new string[] { "Аспирин", "Левомицетин" },
                new string[] { "Грипп в 2020 году", "Аллергический дерматит" }
            ),
            
            new MedicalRecord(
                2,
                "Петрова Мария Сергеевна",
                45,
                "A-",
                "Пищевые добавки",
                new string[] { "Кальций", "Витамин С" },
                new string[] { "Остеопороз", "Гипертония" }
            ),
            
            new MedicalRecord(
                3,
                "Сидоров Николай Александрович",
                60,
                "O+",
                "Пыльца",
                new string[] { "Аллергин", "Кетотифен" },
                new string[] { "Бронхиальная астма", "Пневмония в 2018 году" }
            )
        };
        
        Blockchain blockchain = new Blockchain(2);
        
        int check = 0;
        
        foreach (var medCard in medCards)
        {
            string data = JsonConvert.SerializeObject(medCard);
            string encryptedData;
            string encryptedKey;
            string encryptedIv;
            
            Console.WriteLine($"Добавление блока: {data}");
        
            if (check == 0)
            {
                byte[] encryptedBytes;
                string publicKey =
                    "<RSAKeyValue><Modulus>vA96FyTLc5xhPkfz6i3Ghe3O3y/qCefPGjpj6rIh5QhryDTsxvjzZoopGxMbCex0KN5hbgSJ9sIzvRD+pRio/lDBqxclXoPJ+mRl3cldhdZFjBl0mrDdcAI6Zktwbd3JfN2uNFW5jd3jql6YsYUKbYYpYq57giWsJ/hL68Fo+N6Chox7+3uH8Lthaiji32mwpTeMNvoo7QKz+vKh1mUhWHGVjRKaNzq4RyqT62H+XFw4lYZf/nPmioG0LuGw/3P2F9IkWEB4RAnwcocq9Xkl+EI/42NMCR9s5SwOD1KW1T9+NhULCrTzZgLCoqHy5xCFABxGNHFcSfk3qrjnZJ09cQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                Encrypt(data, publicKey, out encryptedData, out encryptedKey, out encryptedIv);
                DataBlock dataBlock = new DataBlock("Admin", "Admin", "Создание", encryptedKey, encryptedIv, encryptedData);
                blockchain.AddBlock(new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
                    JsonConvert.SerializeObject(dataBlock), blockchain.GetLatestBlock().Hash));
                check++;
                continue;
            }
        
            Block block = new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
                data, blockchain.GetLatestBlock().Hash);
            blockchain.AddBlock(block);
            check++;
        }
        
        BlockchainFileManager.SaveBlockchain(blockchain, "..\\..\\..\\..\\Diplom.Blockchain\\Source\\blockchain.json");
        
        Blockchain loadedBlockchain = BlockchainFileManager.LoadBlockchain("..\\..\\..\\..\\Diplom.Blockchain\\Source\\blockchain.json");
        
        Console.WriteLine($"Проверка того, действителен ли блокчейн: {blockchain.IsChainValid()}");
        Console.WriteLine($"Проверка правильности загруженного блокчейна: {loadedBlockchain.IsChainValid()}");
        Console.WriteLine($"Блокчейн: {JsonConvert.SerializeObject(blockchain, Formatting.Indented)}");
        
        Block loadedBlock = loadedBlockchain.GetBlockByIndex(1);
        
        DataBlock loadedDataBlock = JsonConvert.DeserializeObject<DataBlock>(loadedBlock.Data);
        
        string privateKey = "<RSAKeyValue><Modulus>vA96FyTLc5xhPkfz6i3Ghe3O3y/qCefPGjpj6rIh5QhryDTsxvjzZoopGxMbCex0KN5hbgSJ9sIzvRD+pRio/lDBqxclXoPJ+mRl3cldhdZFjBl0mrDdcAI6Zktwbd3JfN2uNFW5jd3jql6YsYUKbYYpYq57giWsJ/hL68Fo+N6Chox7+3uH8Lthaiji32mwpTeMNvoo7QKz+vKh1mUhWHGVjRKaNzq4RyqT62H+XFw4lYZf/nPmioG0LuGw/3P2F9IkWEB4RAnwcocq9Xkl+EI/42NMCR9s5SwOD1KW1T9+NhULCrTzZgLCoqHy5xCFABxGNHFcSfk3qrjnZJ09cQ==</Modulus><Exponent>AQAB</Exponent><P>119T+JfBcxAf0WqIWsBebaNneMZITqNsBw4clL+4rtemUMGp8jHWWctOHHNH7GoO0avahXwgMJxnysTcLzmM7Z7vQz+uUtrfSzAee1Vs4u76Zr0fDPh61U79RHivF68kAP1pTGXSM3m4GBq4SNct6iecpjlPqv4EhzKBDPT9XD8=</P><Q>34k2l5x0F5dcReHIX1VXkhqne33/CDhQ9x0Mi/9tEFi8LI3mPoqvzRwkspBkPl7NhAyo1yDtCVOD9PbehmoP0dFOdx7Zla/iq9U3k8VaJDvOMDVoRZOYpkZxYuWceU/jm6UHdSfaTxueBEdPKfCAmomz70JyLDY1cjiHCxCYuk8=</Q><DP>JUQtd3pq0sobd1UDuxBGRppbsR4+LL1CWAYtE+AIyNgvwxF/opTVDjyLi4i3DUVcwxMFgMt1lnO50fA2WUWQCR3TMMO4GkYdFRmCbLzfVnUbhuN6l/f26Sn90PdA9MwtYq52pe2IbbfGDwWwlYoGO9oW1PxduKyzg+FNSzypCmk=</DP><DQ>vD9aiS1JmwBtxbARxS8iszjdKKN/3dVHYgPFqDRwDZ8cwUyyxKKY0FvOD86HjPrbikP7AEiLNhpt+yLXXUz+i4z/zlNdm7BmbJz/0+MUOYVf67teV5GnsQeLv2RsdMExhcbh0+i+8XXpieLfqQsP0pT6whgr/E2ejtVJ7KiKZgM=</DQ><InverseQ>Lhf2gXeLkMo3g9hOEhrPb/3S+o21h9gCWuuKX8LdSWy0ZBPWyZ/eLG6vbJjin+zAcUQRdZSS1ucbV5Y1X0XgUaBNELD7zqFx2Y7cnNZEEM8ZO8WyxXn7xh/G+3DldNi+CtCYCr3U9DmjewSDRHNGAZjmpw6eKJSFkfjO+4WI1i8=</InverseQ><D>I5xiqCNFi1zfZSXG4F9Oqmm/tK+kB8AnjXXlGbolhPM1RbIP7BWUMaST6BaUFir6TArgNC8T2PApT/H55lVnGtE7+yPk5aLbClkcmQTaes96V+8yD2DSbbVeTaSXY5aN4uEvbaWV/3E2/TnwfB0PPnIbQB5+MMTldqQj7D6xm/5jaEZA2Xk9XzNm24PlJVyrcPpU25ZVBQRIS1KC6V4NPjytgGhDhQLQTGnGFYJu0A11bhh1+Wmx4YvnwC5vhpKYPN9zud/jB1jDuJOGDMGR56LUBNgCxFUQmuLooTkNN+VZB9YiPFTu1/T5fl5DSK4MFN+pVS+XaejO5syaWApIqQ==</D></RSAKeyValue>";
        string decryptedData = Decrypt(loadedDataBlock.EncryptedData, privateKey, loadedDataBlock.EncryptedKey, loadedDataBlock.EncryptedIv);
        
        Console.WriteLine("Decrypted data:");
        Console.WriteLine(decryptedData);
        
        MedicalRecord loadedMedicalRecord = JsonConvert.DeserializeObject<MedicalRecord>(decryptedData);
        
        Console.WriteLine("Loaded Medical Record:");
        loadedMedicalRecord.DisplayMedicalRecord();
        
    }
    
    static void Encrypt(string plainText, string publicKey, out string encryptedData, out string encryptedKey, out string encryptedIv)
    {
        using (var aes = new AesCryptoServiceProvider())
        {
            aes.GenerateKey();
            aes.GenerateIV();
            byte[] encryptedBytes;

            using (var encryptor = aes.CreateEncryptor())
            using (var memoryStream = new System.IO.MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(System.Text.Encoding.UTF8.GetBytes(plainText), 0, System.Text.Encoding.UTF8.GetBytes(plainText).Length);
                }
                encryptedBytes = memoryStream.ToArray();
            }
            
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                encryptedKey = Convert.ToBase64String(rsa.Encrypt(aes.Key, true));
                encryptedIv = Convert.ToBase64String(rsa.Encrypt(aes.IV, true));
            }

            encryptedData = Convert.ToBase64String(encryptedBytes);
        }
    }

    // Функция расшифровки данных симметричным ключом
    static string Decrypt(string encryptedText, string privateKey, string encryptedKey, string encryptedIv)
    {
        using (var rsa = new RSACryptoServiceProvider())
        using (var aes = new AesCryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey);
            aes.Key = rsa.Decrypt(Convert.FromBase64String(encryptedKey), true);
            aes.IV = rsa.Decrypt(Convert.FromBase64String(encryptedIv), true);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string decryptedText;

            using (var decryptor = aes.CreateDecryptor())
            using (var memoryStream = new System.IO.MemoryStream(encryptedBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                using (var streamReader = new System.IO.StreamReader(cryptoStream))
                {
                    decryptedText = streamReader.ReadToEnd();
                }
            }

            return decryptedText;
        }
    }
}
