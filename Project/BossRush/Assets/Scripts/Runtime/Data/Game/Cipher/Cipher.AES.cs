using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    public static class AES
    {
        private static RijndaelManaged RijndaelCipher = new RijndaelManaged();

        public static string EncryptI64(long input, string key)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform encryptor = RijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] cipherBytes = memoryStream.ToArray();
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public static long DecryptI64(string input, string key)
        {
            byte[] encryptedData = Convert.FromBase64String(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform decryptor = RijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] bytes = new byte[encryptedData.Length];

                int decryptedCount = cryptoStream.Read(bytes, 0, bytes.Length);
                Debug.Assert(decryptedCount == 8);
                return BitConverter.ToInt64(bytes, 0);
            }
        }

        public static string EncryptI32(int input, string key)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform encryptor = RijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] cipherBytes = memoryStream.ToArray();
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public static int DecryptI32(string input, string key)
        {
            byte[] encryptedData = Convert.FromBase64String(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform decryptor = RijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] bytes = new byte[encryptedData.Length];

                int decryptedCount = cryptoStream.Read(bytes, 0, bytes.Length);
                Debug.Assert(decryptedCount == 4);
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        //AES_128 암호화
        public static string Encrypt(string input, string key)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform encryptor = RijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] cipherBytes = memoryStream.ToArray();
                return Convert.ToBase64String(cipherBytes);
            }
        }

        //AE_S128 복호화
        public static string Decrypt(string input, string key)
        {
            byte[] encryptedData = Convert.FromBase64String(input);
            byte[] salt = Encoding.ASCII.GetBytes(key);

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            using (ICryptoTransform decryptor = RijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] bytes = new byte[encryptedData.Length];

                int decryptedCount = cryptoStream.Read(bytes, 0, bytes.Length);
                return Encoding.Unicode.GetString(bytes, 0, decryptedCount);
            }
        }
    }
}