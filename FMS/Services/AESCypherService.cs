using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesCipherService
{
    private readonly string _encryptionKey = "4f5d7e8b9c1a2b4f7d6a3b6c7e9f0d0a1e4c8d7b9c3f2b6c5a4d3b6e7f1a0d1"; // Clé de 32 octets pour AES-256

    /// <summary>
    /// Convertit une chaîne hexadécimale en tableau d'octets.
    /// </summary>
    /// <param name="hex">La chaîne hexadécimale à convertir.</param>
    /// <returns>Le tableau d'octets correspondant à la chaîne hexadécimale.</returns>
    private byte[] ConvertHexToBytes(string hex)
    {
        int length = hex.Length / 2;
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }

    /// <summary>
    /// Vérifie que la clé est valide (32 octets).
    /// Si la clé est trop longue ou trop courte, elle est ajustée à 32 octets.
    /// </summary>
    /// <returns>La clé valide de 32 octets.</returns>
    private byte[] GetValidKey()
    {
        byte[] keyBytes = ConvertHexToBytes(_encryptionKey);

        // Si la clé est trop longue ou trop courte, on la tronque ou on la complète
        if (keyBytes.Length > 32)
        {
            Array.Resize(ref keyBytes, 32);
        }
        else if (keyBytes.Length < 32)
        {
            Array.Resize(ref keyBytes, 32);
        }

        return keyBytes;
    }

    /// <summary>
    /// Chiffre un mot de passe en utilisant AES avec une clé de 32 octets et un vecteur d'initialisation aléatoire.
    /// </summary>
    /// <param name="plainTextPassword">Le mot de passe en texte clair à chiffrer.</param>
    /// <returns>Le mot de passe chiffré sous forme de chaîne encodée en Base64.</returns>
    public string EncryptPassword(string plainTextPassword)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetValidKey(); // Utiliser une clé de 32 octets
            aesAlg.GenerateIV(); // Générer un IV aléatoire

            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Ajouter l'IV au début du flux

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainTextPassword); // Chiffrer le mot de passe
                }

                return Convert.ToBase64String(msEncrypt.ToArray()); // Retourner le mot de passe chiffré sous forme de chaîne
            }
        }
    }

    /// <summary>
    /// Vérifie si un mot de passe en texte clair correspond à un mot de passe chiffré.
    /// </summary>
    /// <param name="encryptedPassword">Le mot de passe chiffré à vérifier.</param>
    /// <param name="plainTextPassword">Le mot de passe en texte clair à comparer.</param>
    /// <returns>Retourne vrai si les mots de passe correspondent, sinon faux.</returns>
    public bool VerifyPassword(string encryptedPassword, string plainTextPassword)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedPassword); // Convertir le mot de passe chiffré en tableau d'octets

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetValidKey(); // Utiliser une clé de 32 octets
            byte[] iv = new byte[16]; // Extraire l'IV (vecteur d'initialisation)
            Array.Copy(cipherBytes, iv, iv.Length); // Copier l'IV depuis le mot de passe chiffré

            aesAlg.IV = iv; // Affecter l'IV à l'algorithme AES
            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                string decryptedPassword = srDecrypt.ReadToEnd(); // Déchiffrer le mot de passe
                return decryptedPassword == plainTextPassword; // Comparer le mot de passe déchiffré avec celui fourni
            }
        }
    }
}
