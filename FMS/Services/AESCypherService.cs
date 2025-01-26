using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesCipherService
{
    private readonly string _encryptionKey = "maCléSecrèteDeChiffrement1234"; // Une clé secrète, utilisez une clé forte !

    // Méthode pour chiffrer le mot de passe
    public string EncryptPassword(string plainTextPassword)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aesAlg.GenerateIV();

            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Ajout du vecteur d'initialisation
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainTextPassword);
                }

                return Convert.ToBase64String(msEncrypt.ToArray()); // Retourne le mot de passe chiffré sous forme de chaîne
            }
        }
    }

    // Méthode pour déchiffrer un mot de passe
    public bool VerifyPassword(string encryptedPassword, string plainTextPassword)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedPassword);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] iv = new byte[16];
            Array.Copy(cipherBytes, iv, iv.Length); // Extraction du vecteur d'initialisation (IV)

            aesAlg.IV = iv;
            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                string decryptedPassword = srDecrypt.ReadToEnd();
                return decryptedPassword == plainTextPassword; // Comparaison du mot de passe déchiffré avec celui fourni
            }
        }
    }
}
