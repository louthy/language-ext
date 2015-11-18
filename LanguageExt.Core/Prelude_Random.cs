using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LanguageExt
{
    public static partial class Prelude
    {
        readonly static RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
        readonly static byte[] target = new byte[4096];

        /// <summary>
        /// Thread-safe cryptographically strong random number generator
        /// </summary>
        /// <param name="max">Maximum value to return + 1</param>
        /// <returns>A non-negative random number, less than the value specified.</returns>
        public static int random(int max)
        {
            lock (rnd)
            {
                rnd.GetBytes(target);
                return Math.Abs(BitConverter.ToInt32(target, 0)) % max;
            }
        }

        /// <summary>
        /// Thread-safe cryptographically strong random base-64 string generator
        /// </summary>
        /// <param name="count">bytesCount - number of bytes generated that are then 
        /// returned Base64 encoded</param>
        /// <returns>Base64 encoded random string</returns>
        public static string randomBase64(int bytesCount)
        {
            if (bytesCount < 1) throw new ArgumentException($"The minimum value for {nameof(bytesCount)} is 1");
            if (bytesCount > 4096) throw new ArgumentException($"The maximum value for {nameof(bytesCount)} is 4096");

            lock (rnd)
            {
                rnd.GetBytes(target);
                return Convert.ToBase64String(target);
            }
        }
    }
}