using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Cashflow.Common.Services.Cryptography
{
    public class Cryptography : ICryptography
    {
        #region Hash  / salt

        public string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];

            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public string CreateHash(string value, string salt)
        {
            byte[] valueBytes = KeyDerivation.Pbkdf2(
                value,
                Encoding.UTF8.GetBytes(salt),
                KeyDerivationPrf.HMACSHA512,
                10000,
                32
            );

            return Convert.ToBase64String(valueBytes);
        }

        public bool Validate(string value, string salt, string hash)
        {
            return CreateHash(value, salt) == hash;
        }

        #endregion

        #region Encrypt / decrypt

        public string Encrypt(string clearText, string encryptionKey = "T9Qh<6d:[x>pxAGt")
        {
            if (!clearText.IsSet() || !encryptionKey.IsSet())
                return null;

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }
                    clearText = Convert.ToBase64String(ms.ToArray()).Replace("\\","_").Replace("/","-").Replace("+","!");
                }
            }

            return clearText;
        }

        public string Decrypt(string cipherText, string encryptionKey = "T9Qh<6d:[x>pxAGt")
        {
            if (!cipherText.IsSet() || !encryptionKey.IsSet())
                return null;

            cipherText = cipherText.Replace("-", "/").Replace("_","\\").Replace("!","+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        #endregion
    }
}
