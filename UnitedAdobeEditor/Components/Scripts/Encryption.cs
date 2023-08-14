using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Scripts
{
    static class Encryption
    {
        public static byte[] EncryptData(byte[] data, string encryptionKey)
        {
            using (var aes = Aes.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);
                var salt = new byte[] { 0x23, 0x11, 0x99, 0x73 };
                var passwordKey = new Rfc2898DeriveBytes(passwordBytes, salt, 1000, HashAlgorithmName.SHA256);
                aes.Key = passwordKey.GetBytes(aes.KeySize / 8);
                aes.IV = passwordKey.GetBytes(aes.BlockSize / 8);

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] DecryptData(byte[] data, string encryptionKey)
        {
            using (var aes = Aes.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);
                var salt = new byte[] { 0x23, 0x11, 0x99, 0x73 };
                var passwordKey = new Rfc2898DeriveBytes(passwordBytes, salt, 1000, HashAlgorithmName.SHA256);
                aes.Key = passwordKey.GetBytes(aes.KeySize / 8);
                aes.IV = passwordKey.GetBytes(aes.BlockSize / 8);

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }

}
