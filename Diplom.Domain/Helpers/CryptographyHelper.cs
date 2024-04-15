using System.Security.Cryptography;

namespace Diplom.Domain.Helpers;

public static class CryptographyHelper
{
    public static void Encrypt(string plainText, string publicKey, out string encryptedData, out string encryptedKey, out string encryptedIv)
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
    public static string Decrypt(string encryptedText, string privateKey, string encryptedKey, string encryptedIv)
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