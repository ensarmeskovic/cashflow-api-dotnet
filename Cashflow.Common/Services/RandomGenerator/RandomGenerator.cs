using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Cashflow.Common.Services.RandomGenerator
{
    public class RandomGenerator : IRandomGenerator
    {
        public string GenerateRandomAlphanumericString(int length)
        {
            if (length <= 3)
                return string.Empty;

            string text = string.Empty;

            text += GetRandomString(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");

            return text;
        }

        public string GenerateRandomNumberString(int length)
        {
            if (length <= 3)
                return string.Empty;

            string text = string.Empty;

            text += GetRandomString(length, "0123456789");

            return text;
        }

        private static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            char[] characterArray = characterSet.Distinct().ToArray();
            byte[] bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            char[] result = new char[length];
            Random rnd = new Random();
            int randomNumber = rnd.Next(length);
            int randomNumber2 = rnd.Next(52, 61);
            for (int i = 0; i < length; i++)
            {
                if (i == randomNumber)
                {
                    ulong value = (ulong)randomNumber2;
                    result[i] = characterArray[value % (uint)characterArray.Length];
                }
                else
                {
                    ulong value = BitConverter.ToUInt64(bytes, i * 8);
                    result[i] = characterArray[value % (uint)characterArray.Length];
                }
            }

            return new string(result);
        }
    }
}
