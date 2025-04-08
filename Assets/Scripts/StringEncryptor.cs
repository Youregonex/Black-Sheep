using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Youregone.Utils
{
    public static class StringEncryptor
    {
        private static readonly string _encryptionKey = "encblacksheep001"; // 16/24/32

        public static string GetFullName(string name)
        {
            string platform = Application.platform.ToString();
            string deviceModelEncrypted = Encrypt(SystemInfo.deviceModel);
            string encryptedName = $"{platform}_{deviceModelEncrypted}|{name}";

            return encryptedName;
        }

        public static string GetShortName(string encryptedString)
        {
            string result = encryptedString.Split('|')[1];
            return result;
        }

        private static string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] ivBytes = new byte[16];
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private static string Decrypt(string encryptedText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] ivBytes = new byte[16];
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
    }
}